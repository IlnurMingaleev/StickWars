using System;
using System.Collections;
using UnityEngine;

namespace Tools.GameTools
{
    public class CoroutineTimer : MonoBehaviour
    {
        private float _currentSeconds = 0;
        private event Action TimeEnd;
        private event Action<float> TickTimerEvent;
        private bool _pause = false;
        public bool IsReloading { get; private set; }

        public void Init(float seconds, Action timeEnd, Action<float> tickTimerEvent = null)
        {
            _currentSeconds = seconds;
            TimeEnd = timeEnd;
            TickTimerEvent = tickTimerEvent;

        }

        public void AddToExistingTimer(float seconds)
        {
            _currentSeconds += seconds;
        }

        public IEnumerator TimerTick()
        {
            while (_currentSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                _currentSeconds -= 1f;
                TickTimerEvent?.Invoke((float) _currentSeconds);
                if (_pause) break;
            }

            if (_currentSeconds <= 0)
            {
                TimeEnd?.Invoke();
            }

        }

        public void StartTick()
        {
            _pause = false;
            if (_currentSeconds <= 0)
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
            _currentSeconds = 0;

        }

        private void EndTick()
        {
            TimeEnd?.Invoke();
        }

        public void InitAndStart(float seconds, Action timeEnd, Action<float> tickTimerEvent = null)
        {
            Init(seconds,timeEnd,tickTimerEvent);
            StartTick();
        }


    }
}