using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameJamProject.SceneManagement
{
    public interface IFadeStrategy
    {
        UniTask FadeOut(Material fadeMaterial);
        UniTask FadeIn(Material fadeMaterial);
    }
}