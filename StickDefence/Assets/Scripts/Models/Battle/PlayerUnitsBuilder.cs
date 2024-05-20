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
        PlayerUnitView InitPlayerUnit(PlayerUnitTypeEnum playerUnitType, Transform parent, SlotTypeEnum slotTypeEnum);
    }

    public class PlayerUnitsBuilder : MonoBehaviour, IPlayerUnitsBuilderTwo
    {

        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly IPlayer _player;
        
        private int _maxHealth = 1;
        private List<ProjectileView> _projectiles = new();
        private readonly List<PlayerUnitModel> _spawnedUnits   = new();
        private PlayerUnitView _playerUnitView;
        private PlayerUnitModel _playerUnitModel;
        private bool _attackSpeedActive = false;
        private CompositeDisposable _disposable = new CompositeDisposable();
        
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
        
        public void StartAttackAnim()
        {
            _playerUnitView.StartAttackAnim();
        }
        
        public float GetDeltaHealth() => (float)_playerUnitView.HealthCurrent.Value / (float) _maxHealth;

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

        public PlayerUnitView InitPlayerUnit(PlayerUnitTypeEnum unitType, Transform parent, SlotTypeEnum slotType)
        { 
            var unitConfig = _configManager.UnitsStatsSo.DictionaryStickmanConfigs[unitType];

            _playerUnitView = Instantiate(_configManager.PrefabsUnitsSO.PlayerUnitPrefabs[unitType].GO, parent).GetComponent<PlayerUnitView>();
            _playerUnitView.gameObject.SetActive(true);
            _playerUnitModel = new PlayerUnitModel(_playerUnitView, _soundManager, _timerService,unitConfig, _attackSpeedActive);
            _playerUnitModel.SetParentSlotType(slotType);
            _playerUnitModel.InitAttack(CreateProjectile, RemoveProjectile);
            _playerUnitModel.InitSubActive();
            _playerUnitModel.OnModelRemove += PlayerUnitOnModelRemove;
            _spawnedUnits.Add(_playerUnitModel);
            return _playerUnitView;
        }

        private void PlayerUnitOnModelRemove(PlayerUnitModel playerUnitModel)
        {
           _spawnedUnits.Remove(playerUnitModel);
           playerUnitModel.OnModelRemove -= PlayerUnitOnModelRemove;
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
            foreach (var unit in _spawnedUnits)
            {
                unit.OnPlay();
            }
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
        }

        private void OnPauseRunTime()
        {
            foreach (var unit in _spawnedUnits)
            {
                unit.OnPause();
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
    }
}