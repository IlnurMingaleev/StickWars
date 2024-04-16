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
using UniRx;
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
        private readonly List<PlayerUnitModel> _spawnedUnits   = new();
        
        private bool _attackSpeedActive = false;
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        public bool AttackSpeedActive => _attackSpeedActive;
        public List<PlayerUnitModel> SpawnedUnits => _spawnedUnits;
        public void SetAttackSpeedActive(bool value) => _attackSpeedActive = value;
      

        private void OnEnable()
        {
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
            _player.Pumping.GamePerks.ObserveReplace().Subscribe(_ => UpdateDamage()).AddTo(_disposable);
        }

        private void OnDisable()
        {
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
            DestroyStage();
            _disposable.Clear();
        }

        public PlayerViewTwo InitStageLoadBattle(PlayerUnitTypeEnum playerUnitType, Transform parent, SlotTypeEnum slotTypeEnum)
        {
            PlayerViewTwo playerViewTwo = null;
            InitFortress(playerUnitType, parent, slotTypeEnum,out playerViewTwo );
            return playerViewTwo;
        }
        

        private void DestroyStage()
        {
            foreach (var projectileView in _projectiles)
            {
                if (projectileView != null)
                {
                    projectileView.DisposeTopDownMove();
                    Destroy(projectileView.gameObject);
                }
            }
            _projectiles.Clear();
        }

        private void InitFortress(PlayerUnitTypeEnum unitType, Transform parent,SlotTypeEnum slotType, out PlayerViewTwo playerViewTwo)
        { 
            var unitConfig = _configManager.UnitsStatsSo.DictionaryStickmanConfigs[unitType];
            var unitView = Instantiate(_configManager.PrefabsUnitsSO.PlayerUnitPrefabs[unitType].GO,parent).GetComponent<PlayerViewTwo>();
            
            var playerUnitModel= new PlayerUnitModel(unitView, _soundManager, _timerService,unitConfig, _attackSpeedActive);
            playerUnitModel.SetParentSlotType(slotType);
            playerUnitModel.InitAttack(CreateProjectile, RemoveProjectile);
            playerUnitModel.InitSubActive();
            playerUnitModel.OnModelRemove += FortressModelOnModelRemove;
            _spawnedUnits.Add(playerUnitModel);
            playerViewTwo = unitView;
        }

        private void FortressModelOnModelRemove(PlayerUnitModel playerUnitModel)
        {
           _spawnedUnits.Remove(playerUnitModel);
           playerUnitModel.OnModelRemove -= FortressModelOnModelRemove;
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
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPlay();
            }
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
        }

        private void OnPauseRunTime()
        {
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPause();
            }

            foreach (var projectileView in _projectiles)
            {
                projectileView.StopMove();
            }  
        }

        public void UpdateDamage()
        {
            foreach (var playerUnitModel in _spawnedUnits)
            {
                int oldDamage = playerUnitModel.RangeAttackModel.GetDamage();
                int newDamage = (int) (oldDamage * (1 +_player.Pumping.GamePerks[PerkTypesEnum.RecruitsDamage].Value/100));
                    playerUnitModel.RangeAttackModel.SetDamage(newDamage);
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