using System;
using Enums;
using Models.Timers;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UnityEngine.Rendering;
using VContainer;

namespace Models.Battle.Boosters
{
    public abstract class Booster:IDisposable
    {
        protected BoosterManager _boosterManager;
        protected CoroutineTimer _boosterTimer;
        protected IWindowManager _windowManager;
        protected const int _boosterActiveTime = 60000;
        protected BoosterTypeEnum _boosterType;
        protected BottomPanelWindow _bottomPanelWindow;

        protected Booster(BoosterManager boosterManager, CoroutineTimer boosterTimer, IWindowManager windowManager)
        {
            _boosterManager = boosterManager;
            _boosterTimer = boosterTimer;
            _windowManager = windowManager;
        }
        public float BoostersActiveTime => _boosterActiveTime;

        public void CreateNewTimerModel(Action timerEndAction, BoosterTypeEnum boosterType)
        {
            _boosterType = boosterType;
            _boosterTimer.InitAndStart( _boosterActiveTime,
                () =>
                {
                    SwitchBoosterOff();
                    timerEndAction?.Invoke();
                    DeactivateTimerUI();
                },
                f =>
                {
                    _windowManager.GetWindow<BottomPanelWindow>().SetTimer(f, _boosterType);
                   
                });
            SwitchBoosterOn();
        }

        public void UpdateExistingTimer()
        {
           _boosterTimer.AddToExistingTimer(_boosterActiveTime);
        }

        public void Dispose()
        {
            _boosterTimer.FinishTimer();
            SwitchBoosterOff();
        }
        public void DeactivateTimerUI()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            _bottomPanelWindow.BoosterDictionary[_boosterType].gameObject.SetActive(false);
            _bottomPanelWindow.RemoveBooster(_boosterType);
        }

        public abstract void SwitchBoosterOn(); 
        public abstract void SwitchBoosterOff();

        public void OnPlay()
        {
            SwitchBoosterOn();
            _boosterTimer.StartTick();
        }

        public void OnPause()
        {
            SwitchBoosterOff();
            _boosterTimer.Pause();
        }

    }
}