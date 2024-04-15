using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UnityEngine;

namespace Models.Timers
{
    public interface ITimerService
    {
        ITimerModel AddGameTimer(float currentSec, Action<float> timeModelTick,
            Action timeModelEndEvent, bool ignoreTimeScale = true);
        ITimerModel AddDefaultTimer(float currentSec, Action<float> timeModelTick, Action timeModelEndEvent);
        void RestartGameTimers();
        void RestartDefaultTimers();
        
    } 
    public class TimerService : ITimerService
    {
        private List<TimerModel> _gameTimeModels = new List<TimerModel>();
        private List<TimerModel> _defaultTimeModels = new List<TimerModel>();

        private List<TimerModel> _tmpTimeModels = new List<TimerModel>();
        private readonly ICoreStateMachine _coreStateMachine;
        public TimerService(ICoreStateMachine coreStateMachine)
        {
            _coreStateMachine = coreStateMachine;
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, OnRunTimePlay);
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, OnRunTimePause);
        }

        private void OnRunTimePlay()
        {
            foreach (var gameTimeModels in _gameTimeModels)
            {
                gameTimeModels.PauseTick();
                gameTimeModels.StartTick();
            }
        }

        private void OnRunTimePause()
        {
            foreach (var gameTimeModels in _gameTimeModels)
            {
                gameTimeModels.PauseTick();
            }
        }

        public ITimerModel AddGameTimer(float currentSec, Action<float> timeModelTick, Action timeModelEndEvent, bool ignoreTimeScale = true)
        {
            if (_tmpTimeModels.Count >= 1)
            {
                var tmpModel = _tmpTimeModels.First();
                _tmpTimeModels.RemoveAt(0);
                
                tmpModel.Init(currentSec, TimerTypeEnum.Game, timeModelTick, timeModelEndEvent, ignoreTimeScale);

                if (_coreStateMachine.RunTimeStateMachine.RunTimeState.Value == RunTimeStateEnum.Play)
                {
                    tmpModel.StartTick();
                }
                _gameTimeModels.Add(tmpModel);
                return tmpModel;
            }
            else
            {
                TimerModel timeModel = new TimerModel(this, currentSec, TimerTypeEnum.Game, timeModelTick, timeModelEndEvent, ignoreTimeScale);
                if (_coreStateMachine.RunTimeStateMachine.RunTimeState.Value == RunTimeStateEnum.Play)
                {
                    timeModel.StartTick();
                }
                _gameTimeModels.Add(timeModel);
                return timeModel;
            }
        }
        public ITimerModel AddDefaultTimer(float currentSec, Action<float> timeModelTick, Action timeModelEndEvent)
        {
            if (_tmpTimeModels.Count >= 1)
            {
                var tmpModel = _tmpTimeModels.First();
                _tmpTimeModels.RemoveAt(0);
                tmpModel.Init(currentSec, TimerTypeEnum.Default, timeModelTick, timeModelEndEvent, true);
                tmpModel.StartTick();
                _defaultTimeModels.Add(tmpModel);
                return tmpModel;
            }
            else
            {
                TimerModel timeModel = new TimerModel(this, currentSec, TimerTypeEnum.Default, timeModelTick, timeModelEndEvent, true);
                timeModel.StartTick();
                _defaultTimeModels.Add(timeModel);
                return timeModel;
            }
        }

        public void RestartGameTimers()
        {
            foreach (var gameTimeModel in _gameTimeModels)
            {
                gameTimeModel.StopTickNoCash();
                _tmpTimeModels.Add(gameTimeModel);
            }
            _gameTimeModels.Clear();

        }
        
        public void RestartDefaultTimers()
        {
            foreach (var defaultTimeModel in _defaultTimeModels)
            {
                defaultTimeModel.StopTickNoCash();
                _tmpTimeModels.Add(defaultTimeModel);
            }
            _defaultTimeModels.Clear();
        }
        public void RemoveTimer(TimerTypeEnum timerTypeEnum, TimerModel timeModel)
        {
            switch (timerTypeEnum)
            {
                case TimerTypeEnum.Game:
                    _gameTimeModels.Remove(timeModel);
                    _tmpTimeModels.Add(timeModel);
                    break;
                case TimerTypeEnum.Default:
                    _defaultTimeModels.Remove(timeModel);
                    _tmpTimeModels.Add(timeModel);
                    break;
            }
        }
       
    }
}