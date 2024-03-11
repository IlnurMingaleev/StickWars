using System.Threading;
using Cysharp.Threading.Tasks;
using UI.UIManager;

namespace UI.Helpers.Transitions.Async
{
    public interface IWindowShowTransitionAsync
    {
        UniTask ShowAsync(Window window, CancellationToken cancellationToken = default);
    }

    public interface IWindowHideTransitionAsync
    {
        UniTask HideAsync(Window window, CancellationToken cancellationToken = default);
    }
}