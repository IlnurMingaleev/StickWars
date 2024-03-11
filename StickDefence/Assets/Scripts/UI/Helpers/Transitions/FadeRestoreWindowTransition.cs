using System;
using Cysharp.Threading.Tasks;
using UI.Helpers.Transitions.Components;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class FadeRestoreComponent : IWindowTransitionComponent
    {
        private float _targetAlpha = 1.0f;
        public FadeRestoreComponent(float targetAlpha)
        {
            _targetAlpha = targetAlpha;
        }

        public void Show(Window window, Action onShown)
        {
            if (window.CanvasGroup)
                window.CanvasGroup.alpha = _targetAlpha;
            onShown();
        }

        public async void Hide(Window window, Action onHidden)
        {
            await UniTask.DelayFrame(1); //Bug on level start. Wait for new component active, then start hiding(no other way to fix this)
            if (window.CanvasGroup)
                window.CanvasGroup.alpha = 0.0f;
            onHidden();
        }
    }
}