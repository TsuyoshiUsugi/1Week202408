using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamProject.SceneManager;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamProject.SceneManagement
{
    public class SceneChangeView : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingUI;
        [SerializeField] private Material _fadeMaterial;
        [SerializeField] private Texture _maskTexture;
        [SerializeField] private Slider _slider;

        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");

        public Material FadeMaterial => _fadeMaterial;
        

        private void Start()
        {
            _fadeMaterial.SetTexture(MaskTex, _maskTexture);
        }

        public void SetLoadingUIActive(bool isActive)
        {
            _loadingUI.SetActive(isActive);
        }

        public async UniTask FadeOut(IFadeStrategy fadeStrategy)
        {
            if (fadeStrategy != null)
            {
                // FadeStrategyからフェードアウトを実行
                await fadeStrategy.FadeOut(_fadeMaterial);
            }
        }

        public async UniTask FadeIn(IFadeStrategy fadeStrategy)
        {
            if (fadeStrategy != null)
            {
                // FadeStrategyからフェードインを実行
                await fadeStrategy.FadeIn(_fadeMaterial);
            }
        }

        public void UpdateProgress(float progress)
        {
            _slider.value = progress;
        }
    }
}