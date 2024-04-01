using System;
using System.Collections.Generic;
using Enums;
using Models.Merge;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Move;
using Models.Player;
using Models.SO.Core;
using Models.Timers;
using Models.Units;
using Models.Units.Units;
using TonkoGames.Sound;
using UI.UIManager;
using UI.Windows;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Views.Move;
using Views.Projectiles;
using Views.Units.Units;
using Random = UnityEngine.Random;

namespace Models.Battle
{
    public class MapUnitsBuilder : MonoBehaviour
    {
        [SerializeField] private MovementPathGroups _meleeMovementPathGroups;
        [SerializeField] private Transform _parentSpawnPoint;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private IWindowManager _windowManager;
        private IBattleStateMachine BattleStateMachine => _coreStateMachine.BattleStateMachine;
        private IRunTimeStateMachine RunTimeStateMachine => _coreStateMachine.RunTimeStateMachine;

        private readonly List<BaseUnit> _spawnedUnits   = new();
        private Queue<MapStageDayGroupConfig> DayGroups = new();
        private ReactiveProperty<bool> _isEmptyDay      = new();
        private List<ProjectileView> _projectiles = new();
        private List<ProjectileView> _playerProjectiles = new();
        private ReactiveProperty<int> _unitsCount = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<bool> IsEmptyDay => _isEmptyDay;

        private ITimerModel _timerDayGroups;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _updateDisposable = new CompositeDisposable();
        private TopPanelWindow _topPanelWindow;
        private int _maxUnitsCount = 0;
        private void OnEnable()
        {
            BattleStateMachine.SubscriptionAction(BattleStateEnum.StartBattle, OnStartBattleState);
            RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, OnPlay);
            RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, OnPause);
        }

        private void OnDisable()
        {
            BattleStateMachine.UnSubscriptionAction(BattleStateEnum.StartBattle, OnStartBattleState);
            RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Play, OnPlay);
            RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Pause, OnPause);
            _disposable.Clear();
            _updateDisposable.Clear();
            DestroyStage();
        }

        private void Start()
        {
            /*while (DayGroups.Count > 0)
            {
                var dayGroup = DayGroups.Peek();
                var unitsGroup = _configManager.MapStageSO.UnitGroups[dayGroup.GroupUnitsIndex];
                for (int i = 0; i < unitsGroup.Units.Count; i++)
                {
                    for (int j = 0; j < unitsGroup.Units[i].Count; j++)
                    {
                        _maxUnitsCount++;
                    }
                }
            }*/

            _topPanelWindow = _windowManager.GetWindow<TopPanelWindow>();
            if (_topPanelWindow)
                _unitsCount.Subscribe(unitsCount =>
                {
                    _topPanelWindow.LevelProgressBar.SetBarFiilAmount(unitsCount,
                       _configManager.MapStageSO.WaveUnitsCount.Value);
                }).AddTo(_disposable);
        }

        private void OnStartBattleState()
        {
        }

        private void OnPlay()
        {
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPlay();
            }
            
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
            
            StartUpdateMove();
        }

        private void OnPause()
        {
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPause();
            }
            
            foreach (var projectileView in _projectiles)
            {
                projectileView.StopMove();
            }  
            
            _updateDisposable.Clear();
        }
        
        private void DestroyStage()
        {
            foreach (var projectileView in _projectiles)
            {
                Destroy(projectileView.gameObject);
            }
            _projectiles.Clear();
        }

        
        private void StartUpdateMove()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                foreach (var baseUnit in _spawnedUnits)
                {
                    baseUnit.Update();
                }
            }).AddTo(_updateDisposable);
        }
        
        public void NextDay(List<MapStageDayGroupConfig> groups)
        {
            DayGroups.Clear();

            foreach (var group in groups)
            {
                DayGroups.Enqueue(group);
            }

            CheckIsEmptyDay();
            
            StartSpawnUnits();
        }
        
        private void StartSpawnUnits()
        {
            if (DayGroups.Count == 0)
                return;
                
            var dayGroup = DayGroups.Peek();
            _timerDayGroups = _timerService.AddGameTimer(dayGroup.Delay, null, EndTimerToSpawnGroup, false);
        }

        private void EndTimerToSpawnGroup()
        {
            InstantiateUnits();
            StartSpawnUnits();
        }

        private void InstantiateUnits()
        {
            var dayGroup = DayGroups.Dequeue();
            var unitsGroup = _configManager.MapStageSO.UnitGroups[dayGroup.GroupUnitsIndex];

            for (int i = 0; i < unitsGroup.Units.Count; i++)
            {
                for (int j = 0; j < unitsGroup.Units[i].Count; j++)
                {
                    var delay = (i + j + 1) * unitsGroup.Units[i].Delay;
                    InstantiateUnit(unitsGroup.Units[i].UnitType, GetUnitMovementPath(unitsGroup.Units[i].MovementType), delay);
                }
            }
        }

        private void InstantiateUnit(UnitTypeEnum unitType, MovementPath movementPath, float delay)
        {
            var unitConfig = _configManager.UnitsStatsSo.EnemyUnitConfigs[unitType];
            
            var unitView = Instantiate(_configManager.PrefabsUnitsSO
                .UnitPrefabs[unitType], movementPath.SpawnPoint.position, Quaternion.identity, _parentSpawnPoint).GetComponent<UnitView>();
            
           
            unitView.gameObject.SetActive(false);
            unitView.SetRandomBodyZ();
            
            unitView.InitUnitMove(movementPath.PathTypes, movementPath.PathElements);
            
            var baseUnit = UnitCreate(unitType, unitView);
            baseUnit.InitActions(UnitKilled);
            baseUnit.InitAttack(CreateProjectile, RemoveProjectile);
            baseUnit.InitUnitConfigStats(unitConfig);
            unitView.SubscribeOnHealthChanged();
            _spawnedUnits.Add(baseUnit);
            
            Observable.Timer (System.TimeSpan.FromSeconds(delay), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe (_ =>
                {
                    unitView.gameObject.SetActive(true);
                    SetupUnitRunTime(baseUnit);
                }).AddTo (_disposable); 
            
        }

        private void UnitKilled(BaseUnit baseUnit)
        {
            Destroy(baseUnit.View.gameObject);
            _spawnedUnits.Remove(baseUnit);
            CheckIsEmptyDay();
            _unitsCount.Value += 1;
        } 
        
        
        private void CheckIsEmptyDay()
        {
            _isEmptyDay.Value = DayGroups.Count == 0 && _spawnedUnits.Count == 0;
        }

        private void SetupUnitRunTime(BaseUnit baseUnit)
        {
            switch (RunTimeStateMachine.RunTimeState.Value)
            {
                case RunTimeStateEnum.Play:
                    baseUnit.OnPlay();
                    break;
                case RunTimeStateEnum.Pause:
                    baseUnit.OnPause();
                    break;
            }
        }

        private MovementPath GetUnitMovementPath(UnitMovementTypeEnum unitTypeEnum)
        {
            switch (unitTypeEnum)
            {
                case UnitMovementTypeEnum.Melee:
                    return _meleeMovementPathGroups.GetRandomPath();
                case UnitMovementTypeEnum.Range:
                    return _meleeMovementPathGroups.GetRandomPath();
                    break;
                case UnitMovementTypeEnum.Mix:
                    break;
            }
            
            return _meleeMovementPathGroups.GetRandomPath();
        }

        private BaseUnit UnitCreate(UnitTypeEnum unitType, UnitView unitView)
        {
            switch (unitType)
            {
                case UnitTypeEnum.Skeleton:
                    return new SkeletonUnitModel(unitView, _timerService, _soundManager);
            }

            return new BaseUnit(unitView, _timerService, _soundManager);
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
    }
}