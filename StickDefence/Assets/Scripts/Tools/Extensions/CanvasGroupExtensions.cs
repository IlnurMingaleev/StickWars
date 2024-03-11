using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Tools.Extensions
{
public static class CanvasGroupExtensions
    {
        private static Tween Fade(this CanvasGroup group, float start, float end, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            //group.alpha = start;
            return group.MyDOFade(start, end, durationMs / 1000f).SetUpdate(UpdateType.Normal, true).SetEase(ease)
                .OnComplete(onComplete);
        }

        public static async UniTask FadeAsync(this CanvasGroup group, float start, float end, float durationMs, Ease ease, 
            CancellationToken coCancellationToken = default(CancellationToken))
        {
            // check new DoTweenAsyncExtensions.cs file to use unitask with dottween
            await group.MyDOFade(start, end, durationMs / 1000f).SetUpdate(UpdateType.Normal, true).SetEase(ease)
            .WaitForCompletion(true).ToUniTask(PlayerLoopTiming.Update, coCancellationToken);
        }

        
        public static TweenerCore<float, float, FloatOptions> MyDOFade(this CanvasGroup target, float start, float endValue, float duration)
        {
            target.alpha = start;
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() =>
            {
                if (target != null)
                    return target.alpha;
                
                return endValue;
            }, x =>
            {
                if (target)
                    target.alpha = x;
            }, endValue, duration);
            t.SetTarget(target);
            return t;
        }
        
        public static void FadeIn(this CanvasGroup group, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            Fade(group, 0f, 1f, durationMs, ease, onComplete);
        }

        public static void FadeOut(this CanvasGroup group, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            Fade(group, 1f, 0f, durationMs, ease, onComplete);
        }

        public static async UniTask FadeInAsync(this CanvasGroup group, float durationMs, Ease ease, 
        CancellationToken cancellationToken = default)
        {
            await FadeAsync(group, 0f, 1f, durationMs, ease, cancellationToken);
        }

        public static async UniTask FadeOutAsync(this CanvasGroup group, float durationMs, Ease ease, 
        CancellationToken cancellationToken = default)
        {
            await FadeAsync(group, 1f, 0f, durationMs, ease, cancellationToken);
        }

        public static void FadeFromCurrent(this CanvasGroup group, float targetValue, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            Fade(group, group.alpha, targetValue, durationMs, ease, onComplete);
        }

        public static Tween FadeInFromCurrent(this CanvasGroup group, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            return Fade(group, group.alpha, 1f, durationMs, ease, onComplete);
        }

        public static Tween FadeOutFromCurrent(this CanvasGroup group, float durationMs, Ease ease, TweenCallback onComplete = null)
        {
            return Fade(group, group.alpha, 0f, durationMs, ease, onComplete);
        }
    }
}