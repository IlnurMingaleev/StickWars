using System;
using UI.UIManager;

namespace UI.Helpers.Transitions.Components
{
    public interface IWindowTransitionComponent
    {
        void Show(Window window, Action onShown);
        void Hide(Window window, Action onHidden);
    }
}