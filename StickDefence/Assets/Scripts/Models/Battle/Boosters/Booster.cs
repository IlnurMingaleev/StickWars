using System;
using Enums;
using Models.Timers;
using UI.UIManager;
using UI.Windows;
using UnityEngine.Rendering;
using VContainer;

namespace Models.Battle.Boosters
{
    public abstract class Booster:IDisposable
    {
        protected BoosterManager _boosterManager;
        protected ITimerService _timerService;
        protected IWindowManager _windowManager;
        protected const float _boosterActiveTime = 60f;
        protected BoosterTypeEnum _boosterType;
        protected ITimerModel _timerModel;
        protected BottomPanelWindow _bottomPanelWindow;

        protected Booster(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager)
        {
            _boosterManager = boosterManager;
            _timerService = timerService;
            _windowManager = windowManager;
        }

        public ITimerModel TimerModel => _timerModel;
        public float BoostersActiveTime => _boosterActiveTime;

        public void CreateNewTimerModel(Action timerEndAction, BoosterTypeEnum boosterType)
        {
            _boosterType = boosterType;
            _timerModel = _timerService.AddGameTimer( _boosterActiveTime,
                f =>
                {
                    _windowManager.GetWindow<BottomPanelWindow>().SetTimer(f, _boosterType);
                },
                () =>
                {
                    SwitchBoosterOff();
                    timerEndAction?.Invoke();
                    DeactivateTimerUI();
                },true);
            
            _timerModel.StartTick();
        }

        public void UpdateExistingTimer()
        {
            _timerModel.AddTimeToExistingTimer(_boosterActiveTime);
        }

        public void Dispose()
        {
            _timerModel.StopTick();
        }
        public void DeactivateTimerUI()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _bottomPanelWindow.BoosterDictionary[_boosterType].gameObject.SetActive(false);
            _bottomPanelWindow.RemoveBooster(_boosterType);
        }

        public abstract void SwitchBoosterOn(); 
        public abstract void SwitchBoosterOff();
    }
}