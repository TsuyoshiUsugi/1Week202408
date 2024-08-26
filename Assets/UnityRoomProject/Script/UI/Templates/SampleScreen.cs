using UnityEngine;
using GameJamProject.UI;

namespace GameJamProject.UI
{
    public class SampleScreen : Node
    {
        public override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("Initializing SampleScreen");
        }

        public override void OnOpenIn()
        {
            base.OnOpenIn();
            Debug.Log("Opening SampleScreen");
        }

        public override void OnCloseIn()
        {
            base.OnCloseIn();
            Debug.Log("Closing SampleScreen");
        }
    }
}