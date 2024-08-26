using System.Collections.Generic;
using UnityEngine;

namespace GameJamProject.UI
{
    public class Node : MonoBehaviour
    {
        public string NodeName;
        public Node ParentNode;
        public List<Node> ChildNodes { get; private set; } = new List<Node>();

        public bool IsActive { get; private set; }

        private void Start()
        {
            // 子オブジェクトにあるすべてのNodeを自動的にChildNodesに登録
            foreach (Transform childTransform in transform)
            {
                Node childNode = childTransform.GetComponent<Node>();
                if (childNode != null)
                {
                    AddChild(childNode);
                }
            }
        }

        public virtual void OnInitialize()
        {
            Debug.Log($"{NodeName}: OnInitialize");
        }

        public virtual void OnOpenIn()
        {
            Debug.Log($"{NodeName}: OnOpenIn");
            gameObject.SetActive(true);
            IsActive = true;
        }

        public virtual void OnCloseIn()
        {
            Debug.Log($"{NodeName}: OnCloseIn");
            gameObject.SetActive(false);
            IsActive = false;
        }

        public virtual void OnOpenOut()
        {
            Debug.Log($"{NodeName}: OnOpenOut");
        }

        public virtual void OnCloseOut()
        {
            Debug.Log($"{NodeName}: OnCloseOut");
        }

        public void Open()
        {
            Debug.Log($"Opening {NodeName}");
            OnOpenIn();

            foreach (var child in ChildNodes)
            {
                child.OnOpenOut();
            }
        }

        public void Close()
        {
            Debug.Log($"Closing {NodeName}");
            OnCloseIn();

            foreach (var child in ChildNodes)
            {
                child.OnCloseOut();
            }
        }

        public void AddChild(Node child)
        {
            if (child == null || ChildNodes.Contains(child)) return;

            ChildNodes.Add(child);
            child.ParentNode = this;
        }

        public void RemoveChild(Node child)
        {
            if (child == null || !ChildNodes.Contains(child)) return;

            ChildNodes.Remove(child);
            child.ParentNode = null;
        }

        public void SwitchTo(Node newChild)
        {
            if (newChild == null) return;

            foreach (var child in ChildNodes)
            {
                child.Close();
            }

            if (!ChildNodes.Contains(newChild))
            {
                AddChild(newChild);
            }

            newChild.Open();
        }
    }
}
