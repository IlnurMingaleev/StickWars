using System;
using Models.Timers;
using UI.UIManager;
using UI.Windows;

namespace Models.Battle.Boosters
{
    public class AutoMerge: Booster
    {
        public AutoMerge(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager) : base(boosterManager, timerService, windowManager)
        {
        }

        public override void ApplyBooster()
        {
            throw new System.NotImplementedException();
        }

        public override void CreateNewTimerModel(Action timerEndAction)
        {
            _timerModel = _timerService.AddGameTimer( _boosterActiveTime,
                f =>
                {
                    _windowManager.GetWindow<BottomPanelWindow>().SetTimer(f, _boosterType);
                    
                },
                () =>
                {
                   //TODO
                    timerEndAction?.Invoke();
                    DeactivateTimerUI();
                },true);
            
            _timerModel.StartTick();
        }
    }
}