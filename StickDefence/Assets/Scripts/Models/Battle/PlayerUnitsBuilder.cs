
using System.Collections.Generic;
using Enums;
using Models.Fortress;
using Models.Merge;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Player;
using Models.Timers;
using Models.Units.Units;
using TonkoGames.Sound;
using UniRx;
using UnityEngine;
using VContainer;
using Views.Projectiles;
using Views.Units.Units;


namespace Models.Battle
{
    public interface IPlayerUnitsBuilder
    {
        PlayerView InstantiateUnit(PlayerUnitTypeEnum unitType, Transform parent, SlotTypeEnum slotType);
    }

    public class PlayerUnitsBuilder : MonoBehaviour, IPlayerUnitsBuilder
    {
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ISoundManager _soundManager;
        private IBattleStateMachine BattleStateMachine => _coreStateMachine.BattleStateMachine;
        private IRunTimeStateMachine RunTimeStateMachine => _coreStateMachine.RunTimeStateMachine;

        private readonly List<PlayerUnitModel> _spawnedUnits   = new();
       // private Queue<MapStageDayGroupConfig> DayGroups = new();
        private ReactiveProperty<bool> _isEmptyDay      = new();
        private List<ProjectileView> _projectiles = new();
        private List<ProjectileView> _playerProjectiles = new();

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
            DestroyStage();
        }

        private void OnStartBattleState()
        {
        }

        private void OnPlay()
        {
            /*
            foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPlay();
            }
            */
            
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
            
        }

        private void OnPause()
        {
            /*foreach (var baseUnit in _spawnedUnits)
            {
                baseUnit.OnPause();
            }*/
            
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


        public PlayerView InstantiateUnit(PlayerUnitTypeEnum unitType, Transform parent,SlotTypeEnum slotType)
        {
            var unitConfig = _configManager.UnitsStatsSo.DictionaryStickmanConfigs[unitType];
            var unitView = Instantiate(_configManager.PrefabsUnitsSO.PlayerUnitPrefabs[unitType].GO,parent).GetComponent<PlayerView>();
            //var baseUnit = UnitCreate(unitType, unitView); ;
           // baseUnit.InitAttack(CreateProjectile, RemoveProjectile);
          
           // _spawnedUnits.Add(baseUnit);
            return unitView;
        }
        

        private void SetupUnitRunTime(BasePlayerUnit baseUnit)
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

     

        /*private PlayerUnitModel UnitCreate(PlayerUnitTypeEnum unitType, PlayerView unitView)
        {
            /*switch (unitType)
            {
                case PlayerUnitTypeEnum.PlayerOne:
                    return new BasePlayerUnitUnitOne(unitView, _timerService, _soundManager);
                case PlayerUnitTypeEnum.PLayerTwo:
                    return new BasePlayerUnitUnitTwo(unitView, _timerService, _soundManager);
                case PlayerUnitTypeEnum.PLayerThree:
                    return new
            }#1#
            
            //return new (unitView,  _soundManager,_timerService, _player.Pumping);
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
    }
}