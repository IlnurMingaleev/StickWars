using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Tools.Blockers;
using Tools.Extensions;
using UI.UIManager;

namespace UI.Helpers.Transitions.Async
{
    public class FadeWindowTransitionAsync : IWindowShowTransitionAsync, IWindowHideTransitionAsync
    {
        private readonly float _duration;
        private readonly float _hideDuration;
        private readonly GlobalWindowBlocker _windowBlocker = null;

        public FadeWindowTransitionAsync(IWindowManager windowManager, float durationMs, float hideDurationMs, bool useBlocker = false)
        {
            _duration = durationMs;
            _hideDuration = hideDurationMs;
            
            if (useBlocker)
                _windowBlocker = new GlobalWindowBlocker(windowManager);
        }
        
        public FadeWindowTransitionAsync(IWindowManager windowManager, float durationMs) : this(windowManager, durationMs, durationMs) { }
        public FadeWindowTransitionAsync(IWindowManager windowManager, float durationMs, bool useBlocker) : this(windowManager, durationMs, durationMs, useBlocker) { }

        public async UniTask ShowAsync(Window window, CancellationToken cancellationToken = default)
        {
            _windowBlocker?.Block();
            //window.CanvasGroup.alpha = 0;
            window.Show();
            await window.CanvasGroup.FadeInAsync(_duration, Ease.Linear, cancellationToken).SuppressCancellationThrow();
            _windowBlocker?.Unblock();
        }

        public async UniTask HideAsync(Window window, CancellationToken cancellationToken = default)
        {
            _windowBlocker?.Block();
            await window.CanvasGroup.FadeOutAsync(_hideDuration, Ease.Linear, cancellationToken).SuppressCancellationThrow();
            window.Hide();
            _windowBlocker?.Unblock();
        }
    }
}