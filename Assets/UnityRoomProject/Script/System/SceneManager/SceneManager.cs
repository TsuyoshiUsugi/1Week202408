using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRoomProject.System;

namespace GameJamProject.SceneManagement
{
    public class SceneManager : Singleton<SceneManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        private Scene _lastScene;
        private SceneLoader _sceneLoader;
        private readonly string _neverUnloadSceneName = "ManagerScene";
        private readonly Stack<string> _sceneHistory = new Stack<string>();

        protected override async void OnAwake()
        {
            base.OnAwake();
            _sceneLoader = new SceneLoader();
            _lastScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            await LoadNeverUnloadSceneAsync();
        }

        /// <summary>
        /// 常にアンロードされないシーンをロードします。
        /// </summary>
        private async UniTask LoadNeverUnloadSceneAsync()
        {
            await _sceneLoader.LoadSceneAsync(_neverUnloadSceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// フェード戦略を使用してシーンをロードします。
        /// </summary>
        /// <param name="sceneName">ロードするシーンの名前</param>
        /// <param name="fadeStrategy">使用するフェード戦略</param>
        public async UniTask LoadSceneWithFade(string sceneName, IFadeStrategy fadeStrategy = null)
        {
            // fadeStrategy が指定されていない場合は BasicFadeStrategy を使用
            fadeStrategy ??= new BasicFadeStrategy();

            SceneChangeView sceneChangeView = FindObjectOfType<SceneChangeView>();

            if (sceneChangeView == null)
            {
                Debug.LogError("SceneChangeViewが見つかりません。");
                return;
            }

            // フェードアウト
            await sceneChangeView.FadeOut(fadeStrategy);

            // ローディング画面を表示
            sceneChangeView.SetLoadingUIActive(true);
            await UniTask.Delay(500); // ローディング画面表示のための短い遅延

            // シーンをロード
            var loadSceneOperation =
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadSceneOperation.allowSceneActivation = false;

            while (loadSceneOperation.progress < 0.9f)
            {
                sceneChangeView.UpdateProgress(loadSceneOperation.progress);
                await UniTask.Yield();
            }

            // シーンをアクティブ化
            loadSceneOperation.allowSceneActivation = true;
            await UniTask.Yield(); // アクティベーションが完了するまで待機

            //前のシーンをアンロード
            if (_lastScene.isLoaded)
            {
                await UnloadScene(_lastScene.name);
            }

            sceneChangeView.SetLoadingUIActive(false);

            // フェードイン
            await sceneChangeView.FadeIn(fadeStrategy);


            _lastScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

            //シーン履歴を記録
            if (!_sceneLoader.IsSceneLoaded(sceneName))
            {
                _sceneHistory.Push(sceneName);
            }
        }

        
        
        /// <summary>
        /// シーンをフェードでリロードします。
        /// </summary>
        /// <param name="fadeStrategy">使用するフェード戦略</param>
        public async UniTask ReloadSceneWithFade(IFadeStrategy fadeStrategy = null)
        {
            // fadeStrategy が指定されていない場合は BasicReloadFadeStrategy を使用
            fadeStrategy ??= new BasicFadeStrategy();

            SceneChangeView sceneChangeView = FindObjectOfType<SceneChangeView>();

            if (sceneChangeView == null)
            {
                Debug.LogError("SceneChangeViewが見つかりません。");
                return;
            }

            string currentSceneName = _lastScene.name;

            // フェードアウト（リロード用）
            await sceneChangeView.FadeOut(fadeStrategy);

            // シーンをリロード
            await UnloadScene(currentSceneName);
            await LoadScene(currentSceneName);

            // フェードイン（リロード用）
            await sceneChangeView.FadeIn(fadeStrategy);
        }
        
        
        /// <summary>
        /// フェード戦略を使用してシーンをアンロードします。
        /// </summary>
        /// <param name="sceneName">アンロードするシーンの名前</param>
        /// <param name="fadeStrategy">使用するフェード戦略</param>
        public async UniTask UnloadSceneWithFade(string sceneName, IFadeStrategy fadeStrategy)
        {
            if (sceneName == _neverUnloadSceneName)
            {
                Debug.LogWarning($"Cannot unload the never unload scene: {sceneName}");
                return;
            }

            SceneChangeView sceneChangeView = FindObjectOfType<SceneChangeView>();

            if (sceneChangeView == null)
            {
                Debug.LogError("SceneChangeViewが見つかりません。");
                return;
            }

            // ローディング画面を表示
            sceneChangeView.SetLoadingUIActive(true);
            await sceneChangeView.FadeOut(fadeStrategy);

            // シーンをアンロード
            await UnloadSceneWithProgress(sceneName, sceneChangeView);

            sceneChangeView.SetLoadingUIActive(false);

            // フェードイン
            await sceneChangeView.FadeIn(fadeStrategy);
        }

        /// <summary>
        /// ロード進行状況を管理しながらシーンをアンロードします。
        /// </summary>
        /// <param name="sceneName">アンロードするシーンの名前</param>
        /// <param name="sceneChangeView">シーン変更時のUI管理クラス</param>
        private async UniTask UnloadSceneWithProgress(string sceneName, SceneChangeView sceneChangeView)
        {
            var unloadSceneOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadSceneOperation.isDone)
            {
                sceneChangeView.UpdateProgress(unloadSceneOperation.progress);
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 前のシーンに戻ります（フェードイン/アウトを使用）。
        /// </summary>
        /// <param name="fadeStrategy">使用するフェード戦略</param>
        public async UniTask LoadPreviousSceneWithFade(IFadeStrategy fadeStrategy)
        {
            if (_sceneHistory.Count > 1)
            {
                string currentScene = _sceneHistory.Pop();
                string previousScene = _sceneHistory.Peek();
                await UnloadSceneWithFade(currentScene, fadeStrategy);
                await LoadSceneWithFade(previousScene, fadeStrategy);
            }
        }

        /// <summary>
        /// シーンをロードし、履歴に追加します。
        /// </summary>
        /// <param name="sceneName">ロードするシーンの名前</param>
        public async UniTask LoadScene(string sceneName)
        {
            await _sceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (!_sceneLoader.IsSceneLoaded(sceneName))
            {
                _sceneHistory.Push(sceneName);
            }

            if (_lastScene.isLoaded)
            {
                await UnloadScene(_lastScene.name);
            }

            _lastScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        }

        /// <summary>
        /// シーンをアンロードします。
        /// </summary>
        /// <param name="sceneName">アンロードするシーンの名前</param>
        public async UniTask UnloadScene(string sceneName)
        {
            if (sceneName == _neverUnloadSceneName)
            {
                Debug.LogWarning($"Cannot unload the never unload scene: {sceneName}");
                return;
            }
            Debug.Log(sceneName);
            await UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            //await _sceneLoader.UnloadSceneAsync(sceneName);
        }
    }
}