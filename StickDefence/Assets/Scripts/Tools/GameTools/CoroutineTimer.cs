using System;
using System.Collections;
using UnityEngine;

namespace Tools.GameTools
{
    public class CoroutineTimer : MonoBehaviour
    {
        private int _currentMilliseconds = 0;
        private event Action TimeEnd;
        private event Action<float> TickTimerEvent;
        private bool _pause = false;
        public bool IsReloading { get; private set; }

        public void Init(int milliseconds, Action timeEnd, Action<float> tickTimerEvent = null)
        {
            _currentMilliseconds = milliseconds;
            TimeEnd = timeEnd;
            TickTimerEvent = tickTimerEvent;

        }

        public IEnumerator TimerTick()
        {
            while (_currentMilliseconds > 0)
            {
                yield return new WaitForSeconds(0.1f);
                _currentMilliseconds -= 100;
                TickTimerEvent?.Invoke((float) _currentMilliseconds);
                if (_pause) break;
            }

            TimeEnd?.Invoke();

        }

        public void StartTick()
        {
            _pause = false;
            if (_currentMilliseconds <= 0)
                return;
            IsReloading = true;
            StartCoroutine(TimerTick());
        }

        public void Pause()
        {
            _pause = true;
        }

        public void FinishTimer()
        {
            IsReloading = false;
            TimeEnd = null;
            _currentMilliseconds = 0;

        }

        private void EndTick()
        {
            TimeEnd?.Invoke();
        }

        public void InitAndStart(int milliseconds, Action timeEnd, Action<float> tickTimerEvent = null)
        {
            Init(milliseconds,timeEnd,tickTimerEvent);
            StartTick();
        }


    }
}