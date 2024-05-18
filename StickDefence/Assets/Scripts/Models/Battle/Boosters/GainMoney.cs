using System;
using Models.Timers;
using Tools.GameTools;
using UI.UIManager;

namespace Models.Battle.Boosters
{
    public class GainMoney: Booster
    {
        private MapUnitsBuilder _mapUnitsBuilder;
        public GainMoney(BoosterManager boosterManager, CoroutineTimer boosterTimer, IWindowManager windowManager,
            MapUnitsBuilder mapUnitsBuilder) : base(boosterManager, boosterTimer, windowManager)
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