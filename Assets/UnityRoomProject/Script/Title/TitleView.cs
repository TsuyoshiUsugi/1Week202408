using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameJamProject.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField] Button _startButton;
    [SerializeReference, SubclassSelector] IFadeStrategy _fadeStrategy;
    // Start is called before the first frame update
    async void Start()
    {
        _startButton.onClick.AddListener(() => GameJamProject.SceneManagement.SceneManager.Instance.LoadSceneWithFade("OmojiPInGameMain", _fadeStrategy).Forget());
    }
}
