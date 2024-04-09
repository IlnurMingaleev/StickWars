using System;
using Models.Timers;
using UI.UIManager;
using UI.Windows;

namespace Models.Battle.Boosters
{
    public class AttackSpeed: Booster
    {
        private PlayerUnitsBuilder _unitsBuilder;
        private PlayerUnitsBuilderTwo _playerUnitsBuilderTwo;
        public AttackSpeed(BoosterManager boosterManager, ITimerService timerService, IWindowManager windowManager, 
            PlayerUnitsBuilderTwo playerUnitsBuilderTwo) : base(boosterManager, timerService, windowManager)
        {
            _playerUnitsBuilderTwo = playerUnitsBuilderTwo;
        }
        
        public override void SwitchBoosterOn()
        {
            _playerUnitsBuilderTwo.SetAttackSpeedActive(true);
            foreach (var playerUnit in _playerUnitsBuilderTwo.SpawnedUnits)
            {
                playerUnit.SetAttackSpeedActive(true);
                playerUnit.SubscribeStatsWhileAttackSpeedActive();
            }
        }

        public override void SwitchBoosterOff()
        {
            _playerUnitsBuilderTwo.SetAttackSpeedActive(false);
            foreach (var playerUnit in _playerUnitsBuilderTwo.SpawnedUnits)
            {
                playerUnit.SetAttackSpeedActive(false);
                playerUnit.SubscribeStatsWhileAttackSpeedActive();
            }
        }
    }
}