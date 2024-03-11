using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tools.GameTools;
using UnityEngine;

namespace Tools
{
public class GlobalMultiTouchCounter
    {
        private static bool _state;
        private static readonly object _lock = new object();

        public static void Set(bool enabled)
        {
            _state = enabled;
            
            UpdateState();
        }

        public static void Reset()
        {
            _state = true;
            lock (_lock)
            {
                //_tokens.ToList().ForEach(token => token.Dispose());
                _tokens.Clear();
            }

            UpdateState();
        }

        private static void UpdateState([CallerMemberName] string caller = null)
        {
            var oldState = Input.multiTouchEnabled;
            
            var count = 0;

            lock (_lock)
            {
                count = _tokens.Count;
            }

            if (count > 0)
            {
                Input.multiTouchEnabled = false;
            }
            else
            {
                Input.multiTouchEnabled = _state;
            }

            if (oldState != Input.multiTouchEnabled)
                Debugger.Log($"{caller??"()"}-> multitouch: {Input.multiTouchEnabled} tokens: {count}");
        }

        private static HashSet<DisableToken> _tokens = new HashSet<DisableToken>();

        private static void AddToken(DisableToken token)
        {
            // lock (_lock)
            // {
            //     _tokens.Add(token);
            // }
            //
            // UpdateState();
        }

        private static void RemoveToken(DisableToken token)
        {
            // lock (_lock)
            // {
            //     _tokens.Remove(token);
            // }
            //
            // UpdateState();
        }
        
        public static DisableToken GetDisableToken()
        {
            return new DisableToken();
        }
        
        public class DisableToken : IDisposable
        {
            private bool _isDisposed;
            public DisableToken()
            {
                AddToken(this);
            }
            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    RemoveToken(this);
                }
                GC.SuppressFinalize(this); 
            }

            ~DisableToken()
            {
                Dispose();
            }
        }
    }
}