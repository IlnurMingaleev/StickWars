using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Models.Timers
{
    public interface ITimerModel
    {
        void StartTick();
        void CloseTick();
        void StopTick();
        void AddTimeToExistingTimer(float time);
    }
    public class TimerModel : ITimerModel
    { 
        private float _currentSec = 0;
        private TimerTypeEnum _timerTypeEnum = TimerTypeEnum.Default;
        private readonly TimerService _timerService;
        private event Action<float> _timeModelTick;
        private event Action _timeModelEnd;

        private bool _ignoreTimeScale = true;
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public TimerModel(TimerService timerService, float currentSec, TimerTypeEnum timerTypeEnum, Action<float> timeModelTick, Action timeModelEnd, bool ignoreTimeScale)
        {
            _timerService = timerService;
            Init(currentSec, timerTypeEnum, timeModelTick, timeModelEnd, ignoreTimeScale);
        }
        
        public void Init(float currentSec, TimerTypeEnum timerTypeEnum, Action<float> timeModelTick, Action timeModelEnd, bool ignoreTimeScale)
        {
            _timeModelTick = timeModelTick;
            _currentSec = currentSec;
            _timeModelEnd = timeModelEnd;
            _timerTypeEnum = timerTypeEnum;
            _ignoreTimeScale = ignoreTimeScale;
        }
        
        public void StartTick()
        {
            _disposable.Clear();
            if (_ignoreTimeScale)
            {
                Observable.Timer (System.TimeSpan.FromMilliseconds(100), Scheduler.MainThreadIgnoreTimeScale)
                    .Repeat()
                    .Subscribe (_ =>
                    {
                        TimerSet();
                    }).AddTo (_disposable); 
            }
            else
            {
                Observable.Timer (System.TimeSpan.FromMilliseconds(100))
                    .Repeat()
                    .Subscribe (_ =>
                    {
                        TimerSet();
                    }).AddTo (_disposable); 
            }

        }

        private void TimerSet()
        {
            _currentSec -= 0.1f;
            if (_currentSec <= 0)
            {
                _currentSec = 0;
                _timeModelEnd?.Invoke();
                _timerService.RemoveTimer(_timerTypeEnum, this);
                _disposable.Clear();
            }
            
            _timeModelTick?.Invoke(_currentSec);
        }
        public void StopTick()
        {
            _disposable.Clear();
        }
        

        public void RestartTick()
        {
            _disposable.Clear();
            _currentSec = 0;
            _timeModelTick?.Invoke(0);
            _timeModelEnd?.Invoke();
        }

        public void CloseTick()
        {
            _timeModelTick = null;
            _timeModelEnd = null;
        } 
        
        public void AddTimeToExistingTimer(float time)
        {
            _currentSec += time;
        }
    }
}