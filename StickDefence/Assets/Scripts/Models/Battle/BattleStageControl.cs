using Anim.Battle;
using Enums;
using Models.Controllers;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.Player;
using Models.SO.Core;
using Models.Timers;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;
using VContainer;

namespace Models.Battle
{
    public class BattleStageControl : MonoBehaviour
    {
        [SerializeField] private PlayerFortressInstantiate _playerFortressInstantiate;
        [SerializeField] private MapUnitsBuilder _mapUnitsBuilder;

        private BattleAnimations _battleAnimations;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ITimerService _timerService;

        private IBattleStateMachine BattleStateMachine => _coreStateMachine.BattleStateMachine;
        private BattleResult _battleResult;
        private CompositeDisposable _activeDisposables = new CompositeDisposable();
        private SceneInstances _sceneInstances;
        public MapStageConfig MapStageConfig;
        private ReactiveProperty<int> _currentDayIndex = new ReactiveProperty<int>(-1);
        public IReadOnlyReactiveProperty<int> CurrentDayIndex => _currentDayIndex;

        private void Awake()
        {
            _sceneInstances = GetComponent<SceneInstances>();
            _battleAnimations = new BattleAnimations(_playerFortressInstantiate, _coreStateMachine);
            _battleResult = new BattleResult(_playerFortressInstantiate, _windowManager, _configManager, _player,
                _dataCentralService, _coreStateMachine, gameObject.GetComponent<SceneInstances>());
        }

        public void OnEnable()
        {
            DefaultStates();

            _mapUnitsBuilder.IsEmptyDay.SkipLatestValueOnSubscribe().Subscribe(AllEnemyUnitsDead).AddTo(_activeDisposables);
            _currentDayIndex.SkipLatestValueOnSubscribe().Subscribe( CheckStartSpawnNextDay).AddTo(_activeDisposables);
            BattleStateMachine.SubscriptionAction(BattleStateEnum.None, OnCloseBattle);
            BattleStateMachine.SubscriptionAction(BattleStateEnum.LoadBattle, OnLoadBattleState);
            BattleStateMachine.SubscriptionAction(BattleStateEnum.LaunchAnim, OnLaunchState);
            BattleStateMachine.SubscriptionAction(BattleStateEnum.StartBattle, OnStartBattleState);
            BattleStateMachine.EndBattle += OnBattleEnd;
        }

        private void OnDisable()
        {
            _activeDisposables.Clear();
            
            BattleStateMachine.SubscriptionAction(BattleStateEnum.None, OnCloseBattle);
            BattleStateMachine.UnSubscriptionAction(BattleStateEnum.LoadBattle, OnLoadBattleState);
            BattleStateMachine.UnSubscriptionAction(BattleStateEnum.LaunchAnim, OnLaunchState);
            BattleStateMachine.UnSubscriptionAction(BattleStateEnum.StartBattle, OnStartBattleState);
            _coreStateMachine.BattleStateMachine.SetBattleState(BattleStateEnum.None);
            BattleStateMachine.EndBattle -= OnBattleEnd;
            _timerService.RestartGameTimers();
        }
        

        private void OnCloseBattle()
        {
        }

        private void OnLoadBattleState()
        {
            MapStageConfig = _configManager.MapStageSO.MapStages[ _dataCentralService.PumpingDataModel.StageLoadType.Value];
            _player.Pumping.BattleLoad();
            _battleResult.OnLoadBattleState();
            _playerFortressInstantiate.InitStageLoadBattle();
            _windowManager.GetWindow<FadeWindow>().OpenFade(OnBattleEndFade);
            _mapUnitsBuilder.CountAllUnits();
            _mapUnitsBuilder.ZeroUnitCount();
        }
        
        private void OnBattleEndFade()
        {
            BattleStateMachine.SetBattleState(BattleStateEnum.LaunchAnim);
        }
        
        private void OnLaunchState()
        {
            _battleAnimations.LaunchAnimation().Forget();
        }
        
        private void OnStartBattleState()
        {
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Play);
            _windowManager.Show<BottomPanelWindow>();
            _windowManager.GetWindow<BottomPanelWindow>().Init(_sceneInstances);
            _player.DailyModel.InitBoosterManager(_sceneInstances.BoosterManager);
            _windowManager.Show<TopPanelWindow>(WindowPriority.TopPanel);

            _currentDayIndex.Value = 0;
           
        }

        private void CheckStartSpawnNextDay(int dayIndex)
        {
            if (dayIndex >= MapStageConfig.Days.Count)
            {
                _coreStateMachine.BattleStateMachine.OnEndBattle(true);
                return;
            }
            
            _mapUnitsBuilder.NextDay(MapStageConfig.Days[dayIndex].Groups);
            
        }

        private void AllEnemyUnitsDead(bool value)
        {
            if (value)
            {
                _currentDayIndex.Value++;
            }
        }

        private void OnBattleEnd(bool value)
        {
            _windowManager.Hide<BottomPanelWindow>();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
            Debugger.Log($"{this} Battle end is win: {value}" );
            
            if (value)
                _battleResult.OnWinEvent();
            else
                _battleResult.OnLoseEvent();
        }

       
        private void DefaultStates()
        {
            _currentDayIndex.Value = -1;
        }
    }
}