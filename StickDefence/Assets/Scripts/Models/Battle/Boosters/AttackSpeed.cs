using System;
using Models.Timers;
using UI.UIManager;

namespace Models.Battle.Boosters
{
    public class AttackSpeed: Booster
    {
        public AttackSpeed(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager) : base(boosterManager, timerService, windowManager)
        {
        }

        public override void ApplyBooster()
        {
            throw new NotImplementedException();
        }

        public override void CreateNewTimerModel(Action timerEndAction)
        {
            throw new NotImplementedException();
        }
    }
}