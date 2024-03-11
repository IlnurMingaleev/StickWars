using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI.Helpers.Transitions;
using UI.Constants;
using UI.UIManager;
using UI.Windows;

namespace Tools.Blockers
{
    public class GlobalWindowBlockerWithWaiter: IWindowBlocker, IDisposable, IAsyncDisposable
    {
        private static int _blockersAmount;
        
        private readonly IWindowManager _windowManager;
        private bool _useFade;

        public GlobalWindowBlockerWithWaiter(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        public void Block(bool useFade = false)
        {
            if (_blockersAmount.Equals(0))
            {
                /*
                var serverWaitingWindow = _windowManager.FindWindow<ServerWaitingWindow>();
                if (serverWaitingWindow != null)
                    serverWaitingWindow.CanvasGroup.alpha = 1;
                */
                _useFade = useFade;
                if (useFade)
                    _windowManager.Show<ServerWaitingWindow>(_windowManager.WindowTransitions.ServerWaitingWithFade, WindowPriority.Blocker);
                else
                    _windowManager.Show<ServerWaitingWindow>(_windowManager.WindowTransitions.ServerWaitingWithoutFade, WindowPriority.Blocker);
            }

            ++_blockersAmount;
        }

        public void Unblock()
        {
            --_blockersAmount;
            if (_blockersAmount.Equals(0))
            {
                if (_useFade)
                    _windowManager.Hide<ServerWaitingWindow>(_windowManager.WindowTransitions.ServerWaitingWithFade);
                else
                    _windowManager.Hide<ServerWaitingWindow>(_windowManager.WindowTransitions.ServerWaitingWithoutFade);
            }
        }

        public void Dispose()
        {
            Unblock();
        }

        public ValueTask DisposeAsync() 
        {
            Unblock();
            if (!_useFade)
                return new ValueTask(UniTask.DelayFrame(1).AsTask());
            else
                return new ValueTask(UniTask.Delay(TimeSpan.FromMilliseconds(DefaultUIConstants.ServerWaitingFadeDurationMs)).ContinueWith(() => UniTask.DelayFrame(1)).AsTask());
        }
    }
}