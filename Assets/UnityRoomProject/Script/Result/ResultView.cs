using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameJamProject.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityRoomProject.Audio;

public class ResultView : MonoBehaviour
{
    [SerializeField] private Text _reachFloorText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _titleButton;
    [SerializeField] private Image _resultImage;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private GameObject _resultViewObjects;
    [SerializeField] private GameObject _buttonObjects;
    [SerializeField] private GameObject _reachFloorTexts;
    [SerializeField] private RectTransform _graveImage;
    [SerializeReference, SubclassSelector] private IFadeStrategy _fadeStrategy;

    private void Start()
    {
        _retryButton.onClick.AddListener(() =>
        {
            SceneManager.Instance.LoadSceneWithFade(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, _fadeStrategy).Forget();
        });

        _titleButton.onClick.AddListener(() =>
        {
            SceneManager.Instance.LoadSceneWithFade("TitleScene", _fadeStrategy).Forget();
        });
        _resultViewObjects.SetActive(false);
    }
    
    public async void ShowResult()
    { 
        AudioManager.Instance.StopSE(SEType.Attackbgm);
        _resultViewObjects.SetActive(true);
        _reachFloorTexts.SetActive(false);
        _graveImage.gameObject.SetActive(false);
        _buttonObjects.SetActive(false);
        _graveImage.GetComponent<RawImage>().DOFade(0, 0).SetLink(_graveImage.gameObject).ToUniTask().Forget();
        _resultImage.DOFade(0, 0).SetLink(_resultImage.gameObject).ToUniTask().Forget();
        await _resultImage.DOFade(1, _fadeDuration).SetLink(_resultImage.gameObject);
        _graveImage.gameObject.SetActive(true);
        await _graveImage.GetComponent<RawImage>().DOFade(1, _fadeDuration).SetLink(_graveImage.gameObject);
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        
        await SetReachFloorText(10);    // ここで引数を変えることで表示する階層を変えることができる
        _buttonObjects.SetActive(true);
    }

    private async UniTask SetReachFloorText(int floor)
    {
        _reachFloorTexts.SetActive(true);
        _reachFloorText.text = "";
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        
        int startValue = 0;
        float duration = 2.0f; // Duration of the animation in seconds

        await DOTween.To(() => startValue, x => 
        {
            startValue = x;
            _reachFloorText.text = $"{startValue}";
        }, floor, duration).SetLink(this.gameObject);
    }
}
