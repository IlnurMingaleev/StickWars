using System;
using System.Collections.Generic;
using System.IO.Pipes;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Fortress;
using Models.Player;
using Models.Timers;
using UI.UIManager;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Views.Projectiles;
using Views.Units.Fortress;

namespace Models.Battle
{
    public class PlayerFortressInstantiate : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly IWindowManager _windowManager;
        
        private int _maxHealth = 1;
        private List<ProjectileView> _projectiles = new();
        
        private FortressView _fortressView;
        private FortressModel _fortressModel;
        public bool IsLaunchIsProgress => _fortressView.IsLaunchIsProgress;

        private void OnEnable()
        {
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
        }

        private void OnDisable()
        {
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
            DestroyStage();
        }
        
        public void InitStageLoadBattle()
        {
            InitFortress();
            _fortressView.StartPrepare();
        }
        
        public void StartLaunchAnim()
        {
            _fortressView.StartLaunchAnim();
        }

        public void Resurrect()
        {
            _fortressModel.Resurrect();
        }
        
        public float GetDeltaHealth() => (float)_fortressView.HealthCurrent.Value / (float) _maxHealth;

        public void DestroyStage()
        {
           
            foreach (var projectileView in _projectiles)
            {
                Destroy(projectileView.gameObject);
            }
            _projectiles.Clear();
        }

        private void InitFortress()
        {
            if(_fortressView != null) Destroy(_fortressView.gameObject);
               // Destroy(_fortressView.gameObject);
            /*if (_spawnPoint.childCount > 0)
            {
                Destroy(_spawnPoint.GetChild(0).gameObject);
            }*/
            _fortressView = Instantiate(_configManager.PrefabsUnitsSO.FortressView, _spawnPoint);
            _fortressModel = new FortressModel(_fortressView, _soundManager, _timerService,
                _player.Pumping, _windowManager,_coreStateMachine);
            _fortressModel.InitBottomPanelButton();
            _fortressModel.InitSubActive();

        }

        private void CreateProjectile(ProjectileView projectileView)
        {
            _projectiles.Add(projectileView);

            switch (_coreStateMachine.RunTimeStateMachine.RunTimeState.Value)
            {
                case RunTimeStateEnum.Play:
                    projectileView.StartMove();
                    break;
                case RunTimeStateEnum.Pause:
                    projectileView.StopMove();
                    break;
            }
        }

        private void RemoveProjectile(ProjectileView projectileView)
        {
            _projectiles.Remove(projectileView);
        }

        private void OnPlayRunTimes()
        {
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
        }

        private void OnPauseRunTime()
        {
            foreach (var projectileView in _projectiles)
            {
                projectileView.StopMove();
            }  
        }
        
        
        // [SerializeField] private BuildMapStage _buildMapStage;
        //
        // [Inject] private readonly ConfigManager _configManager;
        // [Inject] private readonly IPlayerGameStats _playerGameStats;
        // 
        // [Inject] private readonly IDataCentralService _dataCentralService;
        // [Inject] private readonly ITimerService _timerService;
        // [Inject] private readonly ISoundManager _soundManager;
        // [Inject] private readonly IWindowManager _windowManager;
        //
        // private PlayerTankModel _playerTankModel;
        // private TankModelView _tankModelView;
        //
        // private CompositeDisposable _runTimeDisposable = new CompositeDisposable();
        // public event Action LoseGame;
        //
        // public void InitStageLoadBattle()
        // {
        //     var tankPumpingConfigModel = _configManager.PumpingConfigSo.TanksConfigModels[_playerGameStats.PlayerCharacterData.Damage.UnitType]
        //         .TankPumpingDatas[_playerGameStats.PlayerCharacterData.Damage.LevelPumping - 1];
        //     _tankModelView.TankVisual.InitHealthBar(_dataCentralService.SubData);
        //     _tankModelView.InitHealthBar(_timerService, _soundManager, _coreStateMachine.RunTimeState, _playerGameStats.PlayerCharacterData.Damage.Rangefinder);
        //     _playerTankModel = new PlayerTankModel(_tankModelView, _inputController, tankPumpingConfigModel, this, _timerService);
        //     _parallaxBackGround.InitHealthBar(_tankModelView.transform);
        //     _cameraView.InitHealthBar(_tankModelView.transform);
        //     MaxHealth = tankPumpingConfigModel.Health;
        //     
        //     _buildMapStage.InitPlayerTransform(_tankModelView.transform);
        //     _windowManager.GetWindow<GameMapStageWindow>().InitHealthBar(tankPumpingConfigModel.UnitType, _tankModelView.HealthCurrent, tankPumpingConfigModel.Health);
        // }
        //
        // public void StageStartBattle()
        // {
        //     _coreStateMachine.RunTimeState.Subscribe(OnRunState).AddTo(_runTimeDisposable);
        //     _tankModelView.TankVisual.StartEngine();
        // }
        //
        // private void OnRunState(RunTimeStateEnum runTimeStateEnum)
        // {
        //     if (runTimeStateEnum == RunTimeStateEnum.Play)
        //     {
        //         _playerTankModel.StartTrackingPlayerInput();
        //     }
        //     else
        //     {
        //         _playerTankModel.StopTrackingPlayerInput();
        //     }
        // }
        //
        // public void KilledUnit()
        // {
        //     LoseGame?.Invoke();
        // }
        //
    }
}