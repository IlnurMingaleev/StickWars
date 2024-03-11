using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.Extensions
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Graphic image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        public static void SetSpriteOrMakeTransparent(this Image image, Sprite sprite)
        {
            image.sprite = sprite;
            image.SetAlpha(sprite == null ? 0f : 1f);
        }

        public static void SetSpriteFromNone(this Image image, Sprite sprite)
        {
            image.sprite = null;
            image.sprite = sprite;
        }

        public static void SetSpriteSync(this Image image, Sprite sprite)
        {
            SetSpriteSync(image, sprite, SetSprite);
        }

        public static void SetSpriteSync(this Image image, Sprite sprite, Action<Image, Sprite> setSpriteAction)
        {
            setSpriteAction(image, sprite);
        }

        public static void SetSpriteAsync(this Image image, Func<CancellationToken, UniTask<Sprite>> loadSpriteAsyncFunc, CancellationToken cancellationToken)
        {
            SetSpriteAsync(image, loadSpriteAsyncFunc, SetSprite, cancellationToken);
        }
        
        public static async void SetSpriteAsync(this Image image, Func<CancellationToken, UniTask<Sprite>> loadSpriteAsyncFunc, Action<Image, Sprite> setSpriteAction, CancellationToken cancellationToken)
        {
            var tuple = await loadSpriteAsyncFunc(cancellationToken).SuppressCancellationThrow();
            if (!tuple.IsCanceled)
                setSpriteAction(image, tuple.Result);
        }
        
        private static void SetSprite(Image image, Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}