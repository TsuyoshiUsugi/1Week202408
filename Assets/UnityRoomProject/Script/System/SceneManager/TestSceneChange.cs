using DG.Tweening;
using UnityEngine;

namespace GameJamProject.SceneManagement
{
    public class TestSceneChange　: MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeReference, SubclassSelector] private IFadeStrategy _fadeStrategy;

        public async void ChangeScene()
        {
            // 例: シーン"GameScene"をフェードでロード
            await SceneManager.Instance.LoadSceneWithFade(
                _sceneName, _fadeStrategy
            );
        }
    }
}