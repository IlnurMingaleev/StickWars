using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tools.Blockers;
using UI.Helpers.Transitions.Async;
using UI.Helpers.Transitions.Components;
using UI.UIManager;

namespace UI.Helpers.Transitions
{
    public class CompositeTransition : IWindowShowTransition, IWindowHideTransition, IWindowShowTransitionAsync
    {
        private readonly IWindowBlocker _blocker;
        private readonly CompositeTransition _next;
        private readonly List<IWindowTransitionComponent> _components;

        public CompositeTransition(IWindowManager windowManager, bool useBlocker, params IWindowTransitionComponent[] components)
        {
            if (useBlocker)
                _blocker = new GlobalWindowBlocker(windowManager);
            _components = components.ToList();
        }

        public void Show(Window window, Action onShown = null)
        {
            window.Show();
            _blocker?.Block();
            int amount = _components.Count;
            foreach (var component in _components)
                component.Show(window, () => { _IsAllFinished(--amount, onShown); });
        }

        public void Hide(Window window, Action onHidden)
        {
            _blocker?.Block();
            int amount = _components.Count;
            foreach (var component in _components)
                component.Hide(window, () => { if (_IsAllFinished(--amount, onHidden)) window.Hide(); });
        }

        private bool _IsAllFinished(int amount, Action action)
        {
            if (amount != 0)
                return false;
            _blocker?.Unblock();
            action?.Invoke();
            return true;
        }

        public UniTask ShowAsync(Window window, CancellationToken cancellationToken = default)
        {
            window.Show();
            _blocker?.Block();
            int amount = _components.Count;
            foreach (var component in _components)
                component.Show(window, () => { _IsAllFinished(--amount, () => {}); });
            return UniTask.WaitWhile(() => amount > 0, cancellationToken: cancellationToken).SuppressCancellationThrow();
        }
    }
}