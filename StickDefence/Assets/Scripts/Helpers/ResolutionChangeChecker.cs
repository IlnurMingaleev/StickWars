using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public static class ResolutionChangeChecker
    {
        #region events
        public static event System.Action ScreenResolutionChanged;
        public static event Action<int> DisplayResolutionChanged;
        #endregion

        #region private fields
        private static Vector2Int _prevScreenResolution;
        private static Vector2Int[] _prevDisplayResolutionArray;
        #endregion

        #region public methods
        public static Vector2Int GetScreenResolution()
        {
            return new Vector2Int(Screen.width, Screen.height);
        }
        
        public static Vector2Int GetDisplayResolution(int index)
        {
            var display = Display.displays[index];

            return GetDisplayResolution(display);
        }

        public static Vector2Int GetDisplayResolution(Display display)
        {
            return new Vector2Int(display.renderingWidth, display.renderingHeight);
        }
        #endregion
        
        #region private methods
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static async void _RuntimeMethodLoadAsync()
        {
            _prevScreenResolution = GetScreenResolution();
            _prevDisplayResolutionArray = Display.displays.Select(GetDisplayResolution).ToArray();

            Display.onDisplaysUpdated += () =>
            {
                _CheckDisplaysResolutionChanged();
            };

            while (true)
            {
                if (!Application.isPlaying)
                {
                    await Task.Delay(1);
                }
                else
                {
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
                }

                _CheckScreenResolutionChanged();

                _CheckDisplaysResolutionChanged();
            }
        }

        private static void _CheckScreenResolutionChanged()
        {
            var oldResolution = _prevScreenResolution;
            var newResolution = GetScreenResolution();
            if (oldResolution != newResolution)
            {
                _prevScreenResolution = newResolution;

                ScreenResolutionChanged?.Invoke();
            }
        }

        private static void _CheckDisplaysResolutionChanged()
        {
            if (_prevDisplayResolutionArray.Length != Display.displays.Length)
            {
                Array.Resize(ref _prevDisplayResolutionArray, Display.displays.Length);
            }

            for (int i = 0; i < Display.displays.Length; i++)
            {
                var display = Display.displays[i];
                var oldDisplayResolution = _prevDisplayResolutionArray[i];
                var newDisplayResolution = GetDisplayResolution(display);

                if (oldDisplayResolution != newDisplayResolution)
                {
                    _prevDisplayResolutionArray[i] = newDisplayResolution;

                    DisplayResolutionChanged?.Invoke(i);
                }
            }
        }
        #endregion
    }
}