using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Tools.Extensions;
using UI.Helpers.Transitions.Components;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class FadeComponent : IWindowTransitionComponent
    {
        private readonly float _durationMs;

        public FadeComponent(float durationMs)
        {
            _durationMs = durationMs;
        }

        public void Show(Window window, Action onShown)
        {
            window.CanvasGroup.FadeIn(_durationMs, Ease.Linear, () => onShown());
        }

        public async void Hide(Window window, Action onHidden)
        {
            await UniTask.DelayFrame(1); //Bug on level start. Wait for new component active, then start hiding(no other way to fix this)
            window.CanvasGroup.FadeOut(_durationMs, Ease.Linear, () => onHidden());
        }
    }
}