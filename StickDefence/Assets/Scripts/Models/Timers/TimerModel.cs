﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Models.Timers
{
    public interface ITimerModel
    {
        void StartTick();
        void StopTick();
        void AddTimeToExistingTimer(float time);
    }
    public class TimerModel : ITimerModel
    { 
        private float _currentSec = 0;
        private TimerTypeEnum _timerTypeEnum = TimerTypeEnum.Default;
        private readonly TimerService _timerService;
        private event Action<float> TimeModelTick;
        private event Action TimeModelEnd;

        private bool _ignoreTimeScale = true;
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        
        public TimerModel(TimerService timerService, float currentSec, TimerTypeEnum timerTypeEnum, Action<float> timeModelTick, Action timeModelEnd, bool ignoreTimeScale)
        {
            _timerService = timerService;
            Init(currentSec, timerTypeEnum, timeModelTick, timeModelEnd, ignoreTimeScale);
        }
        
        public void Init(float currentSec, TimerTypeEnum timerTypeEnum, Action<float> timeModelTick, Action timeModelEnd, bool ignoreTimeScale)
        {
            TimeModelTick = timeModelTick;
            _currentSec = currentSec;
            TimeModelEnd = timeModelEnd;
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
                _timerService.RemoveTimer(_timerTypeEnum, this);
                _disposable.Clear();
                TimeModelEnd?.Invoke();
            }
            
            TimeModelTick?.Invoke(_currentSec);
        }
        public void StopTick()
        {
            StopTickNoCash();
            _timerService.RemoveTimer(_timerTypeEnum, this);
        }
        
        public void PauseTick()
        {
            _disposable.Clear();
        }
        
        public void StopTickNoCash()
        {
            TimeModelTick = null;
            TimeModelEnd = null;
            _disposable.Clear();
            _currentSec = 0;
           
        }

        public void AddTimeToExistingTimer(float time)
        {
            _currentSec += time;
        }
    }
}