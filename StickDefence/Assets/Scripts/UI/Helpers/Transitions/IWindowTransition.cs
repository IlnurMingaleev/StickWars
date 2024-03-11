using System;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public interface IWindowShowTransition
    {
        void Show(Window window, Action onShown = null);
    }

    public interface IWindowHideTransition
    {
        void Hide(Window window, Action onHidden = null);
    }
}