using UI.Constants;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class WindowTransitions
    {
        public WindowTransitions(IWindowManager windowManager)
        {
            Basic                     = new WindowAnimationsTransition(windowManager);
            InstantFadeWithoutBlock   = new CompositeTransition(windowManager, false, new FadeRestoreComponent(1.0f), new AnimationWindowTransition());
            InstantNoFadeWithoutBlock = new CompositeTransition(windowManager, false, new FadeRestoreComponent(0.0f), new AnimationWindowTransition());
            Fade                      = new CompositeTransition(windowManager, true, new FadeComponent(DefaultUIConstants.FadeDurationMs));
            FadeWithoutBlock          = new CompositeTransition(windowManager, false, new FadeComponent(DefaultUIConstants.FadeDurationMs));
            ServerWaitingWithoutFade  = new CompositeTransition(windowManager, false, new FadeRestoreComponent(1.0f), new AnimationWindowTransition());
            ServerWaitingWithFade     = new CompositeTransition(windowManager, false, new FadeComponent(DefaultUIConstants.ServerWaitingFadeDurationMs));
        }
        public readonly WindowAnimationsTransition Basic;
        public readonly CompositeTransition InstantFadeWithoutBlock;
        public readonly CompositeTransition InstantNoFadeWithoutBlock;
        public readonly CompositeTransition Fade;
        public readonly CompositeTransition FadeWithoutBlock;
        public readonly CompositeTransition ServerWaitingWithoutFade;
        public readonly CompositeTransition ServerWaitingWithFade;
    }
}