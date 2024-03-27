using System;
using System.Collections.Generic;
using Enums;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Fortress;
using Models.Merge;
using Models.Player;
using Models.Timers;
using UnityEngine;
using VContainer;
using Views.Projectiles;
using Views.Units.Fortress;
using Views.Units.Units;

namespace Models.Battle
{
    public interface IPlayerUnitsBuilderTwo
    {
        PlayerViewTwo InitStageLoadBattle(PlayerUnitTypeEnum playerUnitType, Transform parent, SlotTypeEnum slotTypeEnum);
    }

    public class PlayerUnitsBuilderTwo : MonoBehaviour, IPlayerUnitsBuilderTwo
    {

        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly IPlayer _player;
        
        private int _maxHealth = 1;
        private List<ProjectileView> _projectiles = new();
        
        private PlayerViewTwo _fortressView;
        private PlayerUnitModel _fortressModel;
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
        
        public PlayerViewTwo InitStageLoadBattle(PlayerUnitTypeEnum playerUnitType, Transform parent, SlotTypeEnum slotTypeEnum)
        {
            PlayerViewTwo playerViewTwo = null;
            InitFortress(playerUnitType, parent, slotTypeEnum,out playerViewTwo );
            _fortressView.StartPrepare();
            return playerViewTwo;
        }
        
        public void StartLaunchAnim()
        {
            _fortressView.StartLaunchAnim();
        }

        public void Resurrect()
        {
           // _playerTankModel.Resurrect();
        }
        
        public float GetDeltaHealth() => (float)_fortressView.HealthCurrent.Value / (float) _maxHealth;

        private void DestroyStage()
        {
            foreach (var projectileView in _projectiles)
            {
                Destroy(projectileView.gameObject);
            }
            _projectiles.Clear();
        }

        private void InitFortress(PlayerUnitTypeEnum unitType, Transform parent,SlotTypeEnum slotType, out PlayerViewTwo playerViewTwo)
        { 
            var unitConfig = _configManager.UnitsStatsSo.DictionaryStickmanConfigs[unitType];
            var unitView = Instantiate(_configManager.PrefabsUnitsSO.PlayerUnitPrefabs[unitType].GO,parent).GetComponent<PlayerViewTwo>();
            _fortressView = unitView;
            _fortressModel = new PlayerUnitModel(unitView, _soundManager, _timerService,unitConfig);
            _fortressModel.SetParentSlotType(slotType);
            _fortressModel.InitAttack(CreateProjectile, RemoveProjectile);
            _fortressModel.InitSubActive();
            playerViewTwo = unitView;
        }
        
        /*public PlayerView InstantiateUnit(PlayerUnitTypeEnum unitType, Transform parent,SlotTypeEnum slotType)
        {
           
            var baseUnit = UnitCreate(unitType, unitView); ;
            baseUnit.InitAttack(CreateProjectile, RemoveProjectile);
          
            _spawnedUnits.Add(baseUnit);
            
            return unitView;
        }*/
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
        //     var tankPumpingConfigModel = _configManager.PumpingConfigSo.TanksConfigModels[_playerGameStats.PlayerCharacterData.Value.UnitType]
        //         .TankPumpingDatas[_playerGameStats.PlayerCharacterData.Value.LevelPumping - 1];
        //     _tankModelView.TankVisual.InitHealthBar(_dataCentralService.SubData);
        //     _tankModelView.InitHealthBar(_timerService, _soundManager, _coreStateMachine.RunTimeState, _playerGameStats.PlayerCharacterData.Value.Rangefinder);
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