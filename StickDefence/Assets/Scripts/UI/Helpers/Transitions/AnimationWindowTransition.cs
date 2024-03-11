using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Animations;
using UI.Helpers.Transitions.Components;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class AnimationWindowTransition : IWindowTransitionComponent
    {
        public AnimationWindowTransition()
        {
        }

        public void Show(Window window, Action onShown)
        {
            int animsAmount = window.WindowAnimations.Count();
            if (animsAmount == 0)
            {
                onShown?.Invoke();
                return;
            }
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween.OnComplete(() => {
                    windowAnimation.tween.OnComplete(null);
                    if (--animsAmount != 0) return;

                    onShown?.Invoke();
                });
            }

        }

        public async void Hide(Window window, Action onHidden)
        {
            await UniTask.DelayFrame(1); //Bug on level start. Wait for new component active, then start hiding(no other way to fix this)
            foreach (WindowAnimation windowAnimation in window.WindowAnimations)
            {
                windowAnimation.tween?.Complete(true);
            }

            onHidden?.Invoke();
        }
    }
}