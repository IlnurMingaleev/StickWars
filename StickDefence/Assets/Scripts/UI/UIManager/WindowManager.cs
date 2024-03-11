using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Models.Fabrics;
using SRF;
using Tools.GameTools;
using UI.Helpers.Transitions;
using UI.Helpers.Transitions.Async;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.UIManager
{
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        [SerializeField] private Canvas _menuCanvas;
        [SerializeField] private Transform _nonActiveParent;
        
        private WindowStack _stack;
        private IWindowFinder _finder;

        public WindowTransitions WindowTransitions { get; private set; }
        public WindowTransitionsAsync WindowTransitionsAsync { get; private set;}
        
        public Canvas MenuCanvas => _menuCanvas;
        public ReadOnlyReactiveProperty<Window> LastWindow => _stack.LastWindow;
        
        [Inject] private readonly PrefabInject _prefabInject;       
        
        private Stack<Window> _lastWindows = new Stack<Window>();
        private ReactiveProperty<int> _lastWindowsCount = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> LastWindowsCount => _lastWindowsCount;

        public void AddCurrentWindow(Window window)
        {
            if (_lastWindows.Count > 0 && _lastWindows.Peek().Equals(window))
            {
                return;
            }
            _lastWindows.Push(window);
            _lastWindowsCount.Value = _lastWindows.Count;
        }

        public void ClearLastWindows()
        {
            _lastWindows.Clear();
            _lastWindowsCount.Value = _lastWindows.Count;
        }
        
        public Window GetLastWindow(bool removeFromStack = true)
        {
            _lastWindowsCount.Value = _lastWindows.Count - 1;
            return removeFromStack ? _lastWindows.Pop() : _lastWindows.Peek();
        }
        
        private void Awake()
        {
            Debugger.Add(this);
            Debugger.Log("Awake");

            WindowTransitions = new WindowTransitions(this);
            WindowTransitionsAsync = new WindowTransitionsAsync(this);
            
            _finder = new WindowFinder(_nonActiveParent, _prefabInject);
            _stack = new WindowStack(_menuCanvas.transform, _nonActiveParent);
        }
        
        public T FindWindow<T>() where T : Window
        {
            T window = _finder.FindWindow<T>();
            return window;
        }
        public T GetWindow<T>() where T : Window
        {
            T window = _finder.GetWindow<T>();
            window.Setup(this);
            return window;
        }
        
        public bool IsShowing(Window window)
        {
            foreach (Window stackVisibleWindow in _stack.VisibleWindows)
            {
                if (stackVisibleWindow == window)
                    return true;
            }
            return false;
        }
        
        public void Show(Window window) => Show(window, WindowTransitions.Basic, window.Priority);
        public void Show(Window window, IWindowShowTransition transition) => Show(window, transition, window.Priority);
        public void Show(Window window, WindowPriority priority) => Show(window, WindowTransitions.Basic, priority);
        public void Show(Window window, IWindowShowTransition transition, WindowPriority priority)
        {
            string wn = window.Name;
            Debugger.Log($"Show window: '{wn}' with priority: '{priority}'");
            if (window.IsShowing)
            {
                Debugger.LogWarning($"Window '{wn}' is already showing");
                return;
            }
            window.Priority = priority;

            _stack.Add(window);
            
            transition.Show(window);
            
            var canvas = window.GetComponent<Canvas>();
            if (canvas != null)
                canvas.overrideSorting = false;
            
            window.gameObject.RemoveComponentIfExists<GraphicRaycaster>();
        }

        public void First(Window window) => First(window, window.Priority);
        public void First(Window window, WindowPriority priority)
        {
            Debugger.Log($"First window in stack '{window.Name}'" );
            window.Priority = priority;
            _stack.First(window);
            WindowTransitions.Basic.Show(window);
        }
        
        public void Hide(Window window) => Hide(window, WindowTransitions.Basic);
        public void Hide(Window window, IWindowHideTransition transition)
        {
            if (window == null) return;
            
            Debugger.Log($"Hide window '{window.Name}'");
            if (!window.IsShowing)
            {
                Debugger.LogWarning("Window '{0}' is already hidden", window.Name);
                return;
            }

            transition.Hide(window, () => _stack.Remove(window));
        }
        
        public T Show<T>() where T : Window => Show<T>(WindowTransitions.Basic);
        public T Show<T>(WindowPriority priority) where T : Window => Show<T>(WindowTransitions.Basic, priority);
        public T Show<T>(IWindowShowTransition transition, WindowPriority? priority = null) where T : Window
        {
            T window = GetWindow<T>();
            Show(window, transition, priority ?? window.Priority);
            return window;
        }
        
        public T First<T>(WindowPriority? priority = null) where T : Window
        {
            T window = GetWindow<T>();
            First(window, priority ?? window.Priority);
            return window;
        }
        
        public T Hide<T>() where T : Window => Hide<T>(WindowTransitions.Basic);
        public T Hide<T>(IWindowHideTransition transition) where T : Window
        {
            T window = GetWindow<T>();
            Hide(window, transition);
            return window;
        }
        
        public void ClearStack()
        {
            Debugger.Log("Clearing stack");
            WindowStack stack = _stack;
            
            stack.Clear();

            foreach (var window in _nonActiveParent.GetComponentsInChildren<Window>(true))
                if (!window.IsUndestroyable)
                    _finder.UnloadWindow(window);
            ClearLastWindows();
        }
        
        public async UniTask ShowAsync(Window window) => await ShowAsync(window, WindowTransitions.Basic, window.Priority);
        public async UniTask ShowAsync(Window window, IWindowShowTransitionAsync transition) => await ShowAsync(window, transition, window.Priority);
        public async UniTask ShowAsync(Window window, WindowPriority priority) => await ShowAsync(window, WindowTransitions.Basic, priority);
        public async UniTask ShowAsync(Window window, IWindowShowTransitionAsync transition, WindowPriority priority, CancellationToken cancellationToken = default)
        {
            string wn = window.Name;
            
            Debugger.Log($"Show window async: '{wn}' with priority: '{priority}'");
            if (IsShowing(window))
            {
                Debugger.LogWarning($"Window '{wn}' is already showing");
                return;
            }
            
            window.Priority = priority;
            _stack.Add(window); //TODO: support cancellation token
            await transition.ShowAsync(window, cancellationToken);
        }

        public UniTask<T> ShowAsync<T>() where T : Window => ShowAsync<T>(WindowTransitions.Basic);
        public UniTask<T> ShowAsync<T>(WindowPriority priority) where T : Window => ShowAsync<T>(WindowTransitions.Basic, priority);
        public async UniTask<T> ShowAsync<T>(IWindowShowTransitionAsync transition, WindowPriority? priority = null, CancellationToken cancellationToken = default) where T : Window
        {
            T window = GetWindow<T>();
            await ShowAsync(window, transition, priority ?? window.Priority, cancellationToken);
            return window;
        }

        public async UniTask HideAsync(Window window) => await HideAsync(window, WindowTransitions.Basic);
        public async UniTask HideAsync(Window window, IWindowHideTransitionAsync transition, CancellationToken cancellationToken = default)
        {
            Debugger.Log($"Hide window async'{window.Name}'");
            if (!IsShowing(window))
            {
                Debugger.LogWarning($"Window '{window.Name}' is already hidden");
                return;
            }
            
            await transition.HideAsync(window, cancellationToken);
            _stack.Remove(window);
        }

        public async UniTask HideAsync<T>() where T : Window => await HideAsync<T>(WindowTransitions.Basic);
        public async UniTask HideAsync<T>(IWindowHideTransitionAsync transition, CancellationToken cancellationToken = default) where T : Window
        {
            await HideAsync(GetWindow<T>(), transition, cancellationToken);
        }

    }
}