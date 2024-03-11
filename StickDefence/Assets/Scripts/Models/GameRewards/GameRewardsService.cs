using System;
using Models.Timers;
using UnityEngine;
using TonkoGames.Controllers.Core;

namespace Models.GameRewards
{
    public class GameRewardsService
    {
        private readonly ITimerService _timerService;
        public bool IsTurretOpen { get; private set; } = false;
        public int SecondTurretTimer { get; private set; } = 0;
        public event Action<bool> IsTurretActiveChange;
        public event Action<int> SecondTimerTurretChange;
        
        public bool IsDroneOpen { get; private set; } = false;
        public int  SecondDroneTimer{ get; private set; } = 0;
        public event Action<bool> IsDroneActiveChange;
        public event Action<int> SecondTimerDroneChange;

        public GameRewardsService(ITimerService timerService)
        {
            _timerService = timerService;
        }

        public void OpenTurret()
        {
            if (IsTurretOpen)
                return;
            IsTurretOpen = true;
            IsTurretActiveChange?.Invoke(true);
    //        OnSecondTimerTurret(ConfigManager.Instance.TimersSO.GameTurretTimer);
       //     _timerService.AddGameTimer(ConfigManager.Instance.TimersSO.GameTurretTimer, OnSecondTimerTurret, EndTurretTimer);
        }

        private void OnSecondTimerTurret(int value)
        {
            SecondTurretTimer = value;
            SecondTimerTurretChange?.Invoke(value);
        }
        private void EndTurretTimer()
        {
            IsTurretOpen = false;
            IsTurretActiveChange?.Invoke(false);
        }
        
        public void OpenDrone()
        {
            if (IsDroneOpen)
                return;
            IsDroneOpen = true;
            IsDroneActiveChange?.Invoke(true);
       //     OnSecondTimerDrone(ConfigManager.Instance.TimersSO.GameDroneTimer);
       //     _timerService.AddGameTimer(ConfigManager.Instance.TimersSO.GameDroneTimer, OnSecondTimerDrone, EndDroneTimer);
        }

        private void OnSecondTimerDrone(int value){
            SecondDroneTimer = value;
            SecondTimerDroneChange?.Invoke(value);
        }
        private void EndDroneTimer()
        {
            IsDroneOpen = false;
            IsDroneActiveChange?.Invoke(false);
        }
    }
}