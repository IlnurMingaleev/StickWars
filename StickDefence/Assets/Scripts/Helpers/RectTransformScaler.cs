using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
 [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformScaler : UIBehaviour, ILayoutSelfController
    {
#pragma warning disable 0649
        #region Inspector
        [SerializeField] private Vector2Int _referenceResolution = new Vector2Int(1136, 640);
        [Range(0, 1f)]
        [SerializeField] private float _matchWidthOrHeight;
        #endregion
#pragma warning restore 0649

        #region private fields
        private RectTransform _rectTransform;

        //private Canvas _canvasRoot;
        
        private DrivenRectTransformTracker _tracker;
        #endregion

        #region properties
        protected RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        #endregion
        
        #region MonoBehaviour
        protected override void OnEnable()
        {
            base.OnEnable();

            //_UpdateCanvas();
            
            _SetDirty();
            
            _tracker.Add(this, RectTransform, DrivenTransformProperties.All);
            
            /*ResolutionChangeChecker.ScreenResolutionChanged += _OnScreenResolutionChanged;
            ResolutionChangeChecker.DisplayResolutionChanged += _OnDisplayResolutionChanged;*/
        }

        protected override void OnDisable()
        {
            /*ResolutionChangeChecker.ScreenResolutionChanged -= _OnScreenResolutionChanged;
            ResolutionChangeChecker.DisplayResolutionChanged -= _OnDisplayResolutionChanged;*/
            
            _tracker.Clear();
            
            _SetDirty();

            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            
            _SetDirty();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            _SetDirty();
        }

        protected override void Reset()
        {
            base.Reset();

            _SetDirty();
        }
#endif
        #endregion

        #region public methods
        public void SetLayoutHorizontal()
        {
            _SetLayoutAxis(0);
        }

        public void SetLayoutVertical()
        {
            _SetLayoutAxis(1);
        }
        #endregion

        #region private methods
        protected virtual float _GetScaleFactor(int axis)
        {
            /*if (_canvasRoot == null)
                return 1f;*/
            
            //logic copied from canvas scaler
            const float logBase = 2;

            /*var screenResolution = ResolutionChangeChecker.GetScreenResolution();
            var targetDisplay = _canvasRoot.targetDisplay;
            if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
            {
                screenResolution = ResolutionChangeChecker.GetDisplayResolution(targetDisplay);
            }*/

            var screenResolution = new Vector2Int(Mathf.Max((int)_GetParentSize(0), 1), Mathf.Max((int)_GetParentSize(1), 1));
            var referenceResolution = _referenceResolution;
            var logWidth = Mathf.Log((float)screenResolution.x / (float)referenceResolution.x, logBase);
            var logHeight = Mathf.Log((float)screenResolution.y / (float)referenceResolution.y, logBase);
            var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, _matchWidthOrHeight);
            
            var scaleFactor = Mathf.Pow(logBase, logWeightedAverage);
            
            return /*1f / _canvasRoot.scaleFactor **/ scaleFactor;
        }

        /*protected virtual Canvas _GetCanvas()
        {
            var parentCanvas = GetComponentInParent<Canvas>();

            return parentCanvas != null ? parentCanvas.rootCanvas : null;
        }

        protected virtual void _UpdateCanvas()
        {
            var oldCanvas = _canvasRoot;
            var newCanvas = _GetCanvas();
            if (oldCanvas != newCanvas)
            {
                _canvasRoot = newCanvas;
                
                _SetDirty();
            }
        }*/

        protected void _SetLayoutAxis(int axis)
        {
            var scaleFactor = _GetScaleFactor(axis);

            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, _GetParentSize(axis) * 1 / scaleFactor);
            
            var localScale = RectTransform.localScale;
            localScale[axis] = scaleFactor;
            RectTransform.localScale = localScale;
        }
        
        protected void _SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }
        
        protected float _GetParentSize(int axis)
        {
            var parent = transform.parent as RectTransform;
            return parent == null ? 0f : parent.rect.size[axis];
        }
        #endregion
        
        #region delegates
        /*protected virtual void _OnDisplayResolutionChanged(int index)
        {
            if (_canvasRoot != null && _canvasRoot.targetDisplay == index)
            {
                _SetDirty();
            }
        }

        protected virtual void _OnScreenResolutionChanged()
        {
             if (_canvasRoot != null && _canvasRoot.targetDisplay == 0)
             {
                _SetDirty();
             }
        }*/

        /*protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();

            _UpdateCanvas();
        }*/
        #endregion
    }
}