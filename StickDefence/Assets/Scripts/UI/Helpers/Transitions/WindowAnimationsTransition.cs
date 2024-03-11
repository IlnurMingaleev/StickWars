using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Tools.Blockers;
using UI;
using UI.Animations;
using UI.Helpers.Transitions.Async;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class WindowAnimationsTransition : IWindowShowTransition, IWindowHideTransition, IWindowShowTransitionAsync, IWindowHideTransitionAsync//, IWindowStackTransition
    {
        private readonly GlobalWindowBlocker _windowBlocker = null;
        private IWindowManager _windowManager;

        public WindowAnimationsTransition(IWindowManager windowManager, bool useBlocker = true)
        {
            _windowManager = windowManager;
            
            if (useBlocker)
                _windowBlocker = new GlobalWindowBlocker(windowManager);
        }

        public void Show(Window window, Action onShown = null)
        {
            window.Show();

            int animsAmount = window.WindowAnimations.Count();
            if (animsAmount == 0)
            {
                onShown?.Invoke();
                return;
            }

            _windowBlocker?.Block();
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween.OnComplete(() => {
                    windowAnimation.tween.OnComplete(null);
                    if (--animsAmount != 0) return;

                    _windowBlocker?.Unblock();
                    onShown?.Invoke();
                });
            }
        }

        public void Hide(Window window, Action onHidden = null)
        {
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween?.Complete(true);
            }

            window.Hide();
            onHidden?.Invoke();
        }

        public async UniTask ShowAsync(Window window, CancellationToken cancellationToken = default)
        {
            using (GlobalWindowBlockerDisposable.SetBlockerWithWaiter(_windowManager))
            {
                if (window is IPreloadResourcesAsync preloadResourcesAsync)
                {
                    await preloadResourcesAsync.PreloadResources(null, cancellationToken);
                }
            }

            window.Show();

            int animsAmount = window.WindowAnimations.Count();
            if (animsAmount == 0) return;

            _windowBlocker?.Block();
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween.OnComplete(() =>
                {
                    windowAnimation.tween.OnComplete(null);
                    --animsAmount;
                });
            }

            var cancelled = await UniTask.WaitWhile(() => animsAmount > 0, cancellationToken: cancellationToken).SuppressCancellationThrow();
            if (cancelled)
                foreach (WindowAnimation windowAnimation in window.WindowAnimations)
                    windowAnimation.DOComplete(true);
            _windowBlocker?.Unblock();
        }

        public async UniTask HideAsync(Window window, CancellationToken cancellationToken = default)
        {
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween?.Complete(true);
            }

            await UniTask.CompletedTask;
            
            window.Hide();
        }
/*
        public void Start(Window toHide, Window toShow, Action onFinished = null)
        {
            toHide.Hide();
            toShow.Show();
            onFinished?.Invoke();
        }
*/        
    }
}