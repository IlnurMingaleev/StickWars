using System;
using System.Collections.Generic;
using Enums;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Move;
using Models.Player;
using Models.SO.Core;
using Models.Timers;
using Models.Units;
using Models.Units.Units;
using UniRx;
using UnityEngine;
using VContainer;
using Views.Move;
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
        private IBattleStateMachine BattleStateMachine => _coreStateMachine.BattleStateMachine;
        private IRunTimeStateMachine RunTimeStateMachine => _coreStateMachine.RunTimeStateMachine;

        private readonly List<BaseUnit> _spawnedUnits   = new();
        private Queue<MapStageDayGroupConfig> DayGroups = new();
        private ReactiveProperty<bool> _isEmptyDay      = new();

        public IReadOnlyReactiveProperty<bool> IsEmptyDay => _isEmptyDay;

        private ITimerModel _timerDayGroups;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _updateDisposable = new CompositeDisposable();

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

            StartUpdateMove();
        }

        private void OnPause()
        {
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPause();
            }
            
            _updateDisposable.Clear();
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
            var unitConfig = _configManager.EnemyUnitsStatsSO.EnemyUnitConfigs[unitType];
            
            var unitView = Instantiate(_configManager.PrefabsUnitsSO
                .UnitPrefabs[unitType], movementPath.SpawnPoint.position, Quaternion.identity, _parentSpawnPoint).GetComponent<UnitView>();
           
            unitView.gameObject.SetActive(false);
            unitView.SetRandomBodyZ();
            
            unitView.InitUnitMove(movementPath.PathTypes, movementPath.PathElements);
            
            var baseUnit = UnitCreate(unitType, unitView);
            baseUnit.InitActions(UnitKilled);
            baseUnit.InitUnitConfigStats(unitConfig);
            
            _spawnedUnits.Add(baseUnit);
            
            Observable.Timer (System.TimeSpan.FromSeconds(delay), Scheduler.MainThreadIgnoreTimeScale)
                .Repeat()
                .Subscribe (_ =>
                {
                    unitView.gameObject.SetActive(true);
                    SetupUnitRunTime(baseUnit);
                }).AddTo (_disposable); 
            
        }

        private void UnitKilled(BaseUnit baseUnit)
        {
            CheckIsEmptyDay();
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
                    return new SkeletonUnitModel(unitView);
            }

            return new BaseUnit(unitView);
        }
        
        
        // [Inject] private readonly IDataCentralService _dataCentralService;
        // [Inject] private readonly ISoundManager _soundManager;
        // [Inject] private readonly IWindowManager _windowManager;
        //
        // private List<Transform> _roads = new List<Transform>();
        //
        // private List<TankAiUnitView> _unitAiModels = new List<TankAiUnitView>();
        //
        // private Transform _playerTransform;
        // private GameMapStageWindow _gameMapStageWindow;
        // private int _killedUnits = 0;
        // public event Action WinEvent;
        //
        // private void Awake()
        // {
        //     _gameMapStageWindow = _windowManager.GetWindow<GameMapStageWindow>();
        // }
        //
        // private void OnEnable()
        // {
        //     _coreStateMachine.RunTimeState.TakeUntilDisable(this).Subscribe(RunTimeState);
        // }
        //
        // private void OnDisable()
        // {
        //     DestroyStage();
        // }
        //
        //
        // public void InitPlayerTransform(Transform playerTransform)
        // {
        //     _playerTransform = playerTransform;
        // }
        //
        // public void KilledUnit()
        // {
        //     _killedUnits++;
        //     _gameMapStageWindow.SetAliveEnemy(_unitAiModels.Count - _killedUnits);
        //     if (_killedUnits == _unitAiModels.Count)
        //     {
        //         WinEvent?.Invoke();
        //     }
        // }
        //
        // private void SetUnits()
        // {
        //     _killedUnits = 0;
        //     IReadOnlyList<MapStageUnitModel> mapStageUnitsModel = _configManager.MapStageSo.MapStageUnitsModel[_playerGameStats.StageLoadIndex.Value].MapStageUnitModes;
        //
        //     for (int i = 0; i < mapStageUnitsModel.Count; i++)
        //     {
        //         TankAiUnitView tankModelView = Instantiate(_configManager.PrefabsUnitsSO
        //             .UnitPrefabs[mapStageUnitsModel[i].UnitType], _spawnUnits).GetComponent<TankAiUnitView>();
        //         
        //         tankModelView.transform.localPosition = new Vector3(mapStageUnitsModel[i].Placement, tankModelView.transform.localPosition.y,
        //             tankModelView.transform.localPosition.x);
        //         tankModelView.transform.localScale = new Vector3(-1, 1, 1);
        //         tankModelView.TankModelView.SetTankVisualZ(i * 5);
        //
        //         tankModelView.Init(_configManager.EnemyUnitsStatsSO.EnemyUnitConfigModel[mapStageUnitsModel[0].UnitType], _playerTransform, 
        //             _dataCentralService.SubData, _timerService, _soundManager, _coreStateMachine.RunTimeState, this, _configManager.EnemyUnitsStatsSO.EnemyWeaponsConfigModel[mapStageUnitsModel[0].UnitType]);
        //         _unitAiModels.Add(tankModelView);
        //     }
        //
        //     _gameMapStageWindow.SetAliveEnemy(_unitAiModels.Count - _killedUnits);
        //     RunTimeState(_coreStateMachine.RunTimeState.Value);
        // }
        //
        // private void DestroyStage()
        // {
        //     foreach (var road in _roads)
        //     {
        //         Destroy(road.gameObject);
        //     }
        //     
        //     foreach (var unitAiModel in _unitAiModels)
        //     {
        //         unitAiModel.Pause();
        //         Destroy(unitAiModel.gameObject);
        //     }
        //     
        //     _roads.Clear();
        //     _unitAiModels.Clear();
        // }
        //
        // private void RunTimeState(RunTimeStateEnum runTimeStateEnum)
        // {
        //     switch (runTimeStateEnum)
        //     {
        //         case RunTimeStateEnum.Play:
        //             foreach (var unitAiModel in _unitAiModels)
        //             {
        //                 unitAiModel.Play();
        //             }
        //
        //             break;
        //         case RunTimeStateEnum.Pause:
        //         foreach (var unitAiModel in _unitAiModels)
        //         {
        //             unitAiModel.Pause();
        //         }
        //
        //         break;
        //     }
        // }
    }
}