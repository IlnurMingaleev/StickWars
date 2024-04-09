using System;
using Models.Timers;
using UI.UIManager;

namespace Models.Battle.Boosters
{
    public class GainMoney: Booster
    {
        private MapUnitsBuilder _mapUnitsBuilder;
        public GainMoney(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager,
            MapUnitsBuilder mapUnitsBuilder) : base(boosterManager, timerService, windowManager)
        {
            _mapUnitsBuilder = mapUnitsBuilder;
        }

      

        public override void SwitchBoosterOn()
        {
            _mapUnitsBuilder.SetGainCoins(true);
        }

        public override void SwitchBoosterOff()
        {
            _mapUnitsBuilder.SetGainCoins(false);
        }
    }
}