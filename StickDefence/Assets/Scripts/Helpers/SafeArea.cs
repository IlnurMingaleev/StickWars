using UI.UIManager;
using UnityEngine;

namespace Helpers
{
    public class SafeArea : UIBehaviour
    {
        private Rect _CurrentSafeArea
        {
            get
            {
// #if UNITY_EDITOR
//                 Rect debugSafeArea = Debugging.Debug.SafeArea;
//                 return debugSafeArea.Equals(Rect.zero) ? Screen.safeArea : debugSafeArea;
// #else
//                 return Screen.safeArea;
// #endif
                return Screen.safeArea;
            }
        }

        private Rect _lastSafeArea = Rect.zero;
        private Canvas _canvasRoot;
        private RectTransform _rectTransform;
        private RectTransform RectTransform =>
            _rectTransform == null ? (_rectTransform = GetComponent<RectTransform>()) : _rectTransform;

        #region MonoBehaviour
        protected override void Awake()
        {
            base.Awake();
            
            UpdateCanvasRoot();
        }

        
        protected override void OnEnable()
        {
            base.OnEnable();

            _SafeAreaChanged();
            
            ResolutionChangeChecker.ScreenResolutionChanged += _OnScreenResolutionChanged;
            ResolutionChangeChecker.DisplayResolutionChanged += _OnDisplayResolutionChanged;
        }

        protected override void OnDisable()
        {
            ResolutionChangeChecker.ScreenResolutionChanged -= _OnScreenResolutionChanged;
            ResolutionChangeChecker.DisplayResolutionChanged -= _OnDisplayResolutionChanged;
            
            base.OnDisable();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();

            UpdateCanvasRoot();
        }
        
        protected virtual void Update()
        {
            if (!_lastSafeArea.Equals(_CurrentSafeArea))
                _SafeAreaChanged();
        }
        #endregion
        
        #region private fields
        private void _SafeAreaChanged()
        {
            Rect current = _CurrentSafeArea;
            _lastSafeArea = current;
            _ApplySafeArea();
        }

        private void _ApplySafeArea()
        {
            if (_canvasRoot == null)
                return;
            
            Rect safeArea = _CurrentSafeArea;
            Vector2Int screenResolution = ResolutionChangeChecker.GetScreenResolution();
            
            int targetDisplay = _canvasRoot.targetDisplay;
            if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
            {
                screenResolution = ResolutionChangeChecker.GetDisplayResolution(targetDisplay);
                safeArea = new Rect(0, 0, screenResolution.x, screenResolution.y);
            }
            
            Vector2 anchorMin = safeArea.min;
            Vector2 anchorMax = safeArea.max;
            anchorMin.x /= screenResolution.x;
            anchorMin.y /= screenResolution.y;
            anchorMax.x /= screenResolution.x;
            anchorMax.y /= screenResolution.y;

            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;
        }
        
        protected virtual Canvas GetCanvas()
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();

            return parentCanvas != null ? parentCanvas.rootCanvas : null;
        }

        protected virtual void UpdateCanvasRoot()
        {
            Canvas oldCanvas = _canvasRoot;
            Canvas newCanvas = GetCanvas();
            if (oldCanvas != newCanvas)
            {
                _canvasRoot = newCanvas;
                
                _SafeAreaChanged();
            }
        }
        #endregion
        
        #region delegates
        protected virtual void _OnDisplayResolutionChanged(int index)
         {
             if (_canvasRoot != null && _canvasRoot.targetDisplay == index)
             {
                 _SafeAreaChanged();
             }
         }

         protected virtual void _OnScreenResolutionChanged()
         {
             if (_canvasRoot != null && _canvasRoot.targetDisplay == 0)
             {
                 _SafeAreaChanged();
             }
         }
         #endregion
    }
}