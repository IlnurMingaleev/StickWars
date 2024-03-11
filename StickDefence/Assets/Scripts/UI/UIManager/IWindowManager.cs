using System.Threading;
using Cysharp.Threading.Tasks;
using UI.Helpers.Transitions;
using UI.Helpers.Transitions.Async;
using UniRx;
using UnityEngine;

namespace UI.UIManager
{
    public interface IWindowManager
    {
        WindowTransitions WindowTransitions { get; }
        WindowTransitionsAsync WindowTransitionsAsync { get; }
        void AddCurrentWindow(Window window);
        void ClearLastWindows();
        IReadOnlyReactiveProperty<int> LastWindowsCount { get; }
        Window GetLastWindow(bool removeFromStack = true);
        T FindWindow<T>() where T : Window;
        T GetWindow<T>() where T : Window;
        
        bool IsShowing(Window window);
        
        void Show(Window window);
        void Show(Window window, IWindowShowTransition transition);
        void Show(Window window, WindowPriority priority);
        void Show(Window window, IWindowShowTransition transition, WindowPriority priority);

        void Hide(Window window);
        void Hide(Window window, IWindowHideTransition transition);

        void First(Window window);
        void First(Window window, WindowPriority priority);

        T Show<T>() where T : Window;
        T Show<T>(WindowPriority priority) where T : Window;
        T Show<T>(IWindowShowTransition transition, WindowPriority? priority = null) where T : Window;

        T Hide<T>() where T : Window;
        T Hide<T>(IWindowHideTransition transition) where T : Window;
        
        T First<T>(WindowPriority? priority = null) where T : Window;

        ReadOnlyReactiveProperty<Window> LastWindow { get; }
        Canvas MenuCanvas { get; }

        void ClearStack();

        UniTask ShowAsync(Window window);
        UniTask ShowAsync(Window window, IWindowShowTransitionAsync transition);
        UniTask ShowAsync(Window window, WindowPriority priority);
        UniTask ShowAsync(Window window, IWindowShowTransitionAsync transition, WindowPriority priority, CancellationToken cancellationToken = default);

        UniTask<T> ShowAsync<T>() where T : Window;
        UniTask<T> ShowAsync<T>(WindowPriority priority) where T : Window;
        UniTask<T> ShowAsync<T>(IWindowShowTransitionAsync transition, WindowPriority? priority = null, CancellationToken cancellationToken = default) where T : Window;

        UniTask HideAsync(Window window);
        UniTask HideAsync(Window window, IWindowHideTransitionAsync transition, CancellationToken cancellationToken = default);

        UniTask HideAsync<T>() where T : Window;
        UniTask HideAsync<T>(IWindowHideTransitionAsync transition, CancellationToken cancellationToken = default) where T : Window;
    }
}