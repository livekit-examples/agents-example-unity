using UnityEngine;
using UnityEngine.UIElements;

namespace AgentsExample
{
    [RequireComponent(typeof(UIDocument))]
    public class OverlayViewController : MonoBehaviour
    {
        private VisualElement _root;

        protected VisualElement RootElement => _root;
        public virtual string Name { get; }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Debug.LogError("View name is not set");
                return;
            }

            var document = GetComponent<UIDocument>().rootVisualElement;
            _root = document.Q<VisualElement>(className: CLASS_PREFIX + Name);

            if (_root == null)
            {
                Debug.LogError("View not found in UI document");
                return;
            }
            Configure();
        }

        private void OnDisable()
        {
            if (_root == null) return;
            _root = null;
        }

        protected virtual void Configure() {}

        public void Present()
        {
            _root.style.display = DisplayStyle.Flex;
            _root.AddToClassList(CLASS_VISIBLE);
        }

        public void Dismiss()
        {
            _root.RemoveFromClassList(CLASS_VISIBLE);
            _root.style.display = DisplayStyle.None;
        }

        private const string CLASS_VISIBLE = CLASS_PREFIX + "visible";
        private const string CLASS_PREFIX = "view--";
    }
}