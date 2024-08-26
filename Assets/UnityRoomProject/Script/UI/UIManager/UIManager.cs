using UnityEngine;
using UnityEngine.Serialization;
using UnityRoomProject.System;

namespace GameJamProject.UI
{
    public class UIManager : Singleton<UIManager>
    {
        protected override bool UseDontDestroyOnLoad => true;
        [SerializeField] private string _prefabPath = "Assets/Scripts/UI/Templates/";
        [SerializeField] private Node _rootNode;
        [SerializeField] private Camera _uiCamera;

        public void OpenNode(Node node)
        {
            if (_rootNode == null)
            {
                _rootNode = node;
                _rootNode.OnInitialize();
                _rootNode.Open();
            }
            else
            {
                _rootNode.SwitchTo(node);
            }
        }

        public void CloseNode(Node node)
        {
            if (node == _rootNode)
            {
                _rootNode.Close();
                _rootNode = null;
            }
            else if (node.ParentNode != null)
            {
                node.ParentNode.RemoveChild(node);
                node.Close();
            }
        }

        public void SetRootNode(Node rootNode)
        {
            _rootNode = rootNode;
            _rootNode.OnInitialize();
            _rootNode.Open();
        }
    }
}