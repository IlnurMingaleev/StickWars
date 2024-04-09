using System;
using Models.Timers;
using UI.UIManager;

namespace Models.Battle.Boosters
{
    public class GainMoney: Booster
    {
        public GainMoney(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager) : base(boosterManager, timerService, windowManager)
        {
        }

        public override void ApplyBooster()
        {
            throw new System.NotImplementedException();
        }

        public override void SwitchBoosterOn()
        {
            throw new NotImplementedException();
        }

        public override void SwitchBoosterOff()
        {
            throw new NotImplementedException();
        }
    }
}