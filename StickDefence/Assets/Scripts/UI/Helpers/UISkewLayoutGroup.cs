using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    
[ExecuteInEditMode]
    public class UISkewLayoutGroup : UIBehaviour, ILayoutGroup
    {
        private const int DefaultValue = 10;

        [SerializeField] private int _angle;
        [SerializeField] private ScrollRect _scrollRect;

        private float? _tanAngle;
        private float TanAngle => _tanAngle ??= Mathf.Tan(_angle * Mathf.Deg2Rad);

        private DrivenRectTransformTracker _tracker;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_scrollRect != null)
                _scrollRect.onValueChanged.AddListener(ScrollRect);

            SetChildrensPositions();
        }

        protected override void OnDisable()
        {
            _tracker.Clear();

            if (_scrollRect != null)
                _scrollRect.onValueChanged.RemoveAllListeners();
            
            base.OnDisable();
        }

        public void SetLayoutHorizontal()
        {
            SetChildrensPositions();
        }

        public void SetLayoutVertical()
        {
            SetChildrensPositions();
        }

        private void ScrollRect(Vector2 arg0)
        {
            SetChildrensPositions();
        }
        private void SetChildrensPositions()
        {
            _tracker.Clear();

            foreach (Transform childTransform in transform)
            {
                LayoutElement layoutElement = childTransform.GetComponent<LayoutElement>();
                if (layoutElement != null && layoutElement.ignoreLayout)
                    continue;

                if (!(childTransform is RectTransform childRectTransform))
                    continue;

                if (_scrollRect == null)
                {
                    Vector2 localPosition = childRectTransform.localPosition;
                    localPosition[0] = localPosition.y * TanAngle;
                    childRectTransform.localPosition = localPosition;
                }
                else
                {
                    Vector2 childPosition = childRectTransform.position;
                    Vector3 viewportPosition = _scrollRect.viewport.position;
                    childPosition.x = viewportPosition.x + (childPosition.y - viewportPosition.y) * TanAngle;
                    childRectTransform.position = childPosition;
                }

                _tracker.Add(this, childRectTransform, DrivenTransformProperties.AnchoredPositionX);
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            _angle = DefaultValue;
            OnValidate();
        }

        protected override void OnValidate()
        {
            _tanAngle = Mathf.Tan(_angle * Mathf.Deg2Rad);
            if (enabled)
                SetChildrensPositions();
        }
#endif
    }
}