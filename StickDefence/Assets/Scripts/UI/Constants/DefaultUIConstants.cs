using DG.Tweening;

namespace UI.Constants
{
    public static class DefaultUIConstants
    {
        public static readonly float InactiveButtonAlpha = 0.1f;

        public static readonly float FadeDurationMs = 500f;
        public static readonly float ServerWaitingFadeDurationMs = 100f;
        public static readonly float MoveDurationMs = 200f;

        public static class Offset
        {
            public static readonly float Small = 100f;
            public static readonly float Medium = 250f;
            public static readonly float Large = 500f;
        }

        public static class Carousel
        {
            public static class Appearance
            {
                public const float FadeMs = 400f;
                public const Ease FadeEase = Ease.Linear;
            }

            public static class Particles
            {
                public const float FadeMs = 200f;
                public const Ease FadeEase = Ease.Linear;
            }
        }
    }
}