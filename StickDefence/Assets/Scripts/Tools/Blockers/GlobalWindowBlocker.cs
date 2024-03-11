using System;
using UI.Helpers.Transitions;
using UI.UIManager;
using UI.Windows;

namespace Tools.Blockers
{
    public class GlobalWindowBlocker : IWindowBlocker, IDisposable
    {
        private static int _blockersAmount;

        private readonly IWindowManager _windowManager;
        private bool _useFade;

        public GlobalWindowBlocker(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        public void Block(bool useFade = false)
        {
            if (_blockersAmount.Equals(0))
            {
                _useFade = useFade;
                if (useFade)
                    _windowManager.Show<BlockerWindow>(_windowManager.WindowTransitions.FadeWithoutBlock, WindowPriority.Blocker);
                else
                    _windowManager.Show<BlockerWindow>(_windowManager.WindowTransitions.InstantNoFadeWithoutBlock, WindowPriority.Blocker);
            }

            ++_blockersAmount;
        }

        public void Unblock()
        {
            --_blockersAmount;
            
            if (_blockersAmount.Equals(0))
            {
                if (_useFade)
                    _windowManager.Hide<BlockerWindow>(_windowManager.WindowTransitions.FadeWithoutBlock);
                else
                    _windowManager.Hide<BlockerWindow>(_windowManager.WindowTransitions.InstantNoFadeWithoutBlock);
            }
        }

        public void Dispose()
        {
            Unblock();
        }
    }
}