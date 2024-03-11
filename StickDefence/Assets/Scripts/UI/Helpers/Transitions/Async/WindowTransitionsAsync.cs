using UI.Constants;
using UI.UIManager;

namespace UI.Helpers.Transitions.Async
{
    public class WindowTransitionsAsync
    {
        public WindowTransitionsAsync(IWindowManager windowManager)
        {
            Fade = new FadeWindowTransitionAsync(windowManager, DefaultUIConstants.FadeDurationMs, true);
            FadeWithoutBlock = new FadeWindowTransitionAsync(windowManager, DefaultUIConstants.FadeDurationMs);
            TutorialFadeWithoutBlock = new FadeWindowTransitionAsync(windowManager, DefaultUIConstants.FadeDurationMs, DefaultUIConstants.FadeDurationMs/2);
        }
        
        // public readonly WindowAnimationsTransition Basic = WindowTransitions.Basic;
        // public readonly CompositeTransition BasicWithoutBlock = WindowTransitions.InstantFadeWithoutBlock;
        public readonly FadeWindowTransitionAsync Fade;
        public readonly FadeWindowTransitionAsync FadeWithoutBlock;
        public readonly FadeWindowTransitionAsync TutorialFadeWithoutBlock;
    }
}