using System;
using Models.Timers;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;

namespace Models.Battle.Boosters
{
    public class AttackSpeed: Booster
    {
        private PlayerUnitsBuilder _playerUnitsBuilder;
        public AttackSpeed(BoosterManager boosterManager, CoroutineTimer boosterTimer, IWindowManager windowManager, 
            PlayerUnitsBuilder playerUnitsBuilder) : base(boosterManager, boosterTimer, windowManager)
        {
            _playerUnitsBuilder = playerUnitsBuilder;
        }
        
        public override void SwitchBoosterOn()
        {
            _playerUnitsBuilder.SetAttackSpeedActive(true);
            foreach (var playerUnit in _playerUnitsBuilder.SpawnedUnits)
            {
                playerUnit.SetAttackSpeedActive(true);
                playerUnit.SubscribeStatsWhileAttackSpeedActive();
            }
        }

        public override void SwitchBoosterOff()
        {
            _playerUnitsBuilder.SetAttackSpeedActive(false);
            foreach (var playerUnit in _playerUnitsBuilder.SpawnedUnits)
            {
                playerUnit.SetAttackSpeedActive(false);
                playerUnit.SubscribeStatsWhileAttackSpeedActive();
            }
        }
    }
}