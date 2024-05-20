using System;
using System.Collections.Generic;
using Enums;
using Models.Attacking;
using Models.DataModels;
using Models.Fabrics;
using Models.Merge;
using Models.Timers;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;


namespace Models.Controllers
{
    public class BoxSpawner:MonoBehaviour
    {
        [Inject] private PrefabInject _prefabInject;
        [Inject] private ITimerService _timerService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ConfigManager _configManager;
        [Inject] private IDataCentralService _dataCentralService;
        [Inject] private ICoreStateMachine _coreStateMachine;
        private float _cooldownTime = 30f;


        [Header("MergeController")] [SerializeField]
        private MergeController _mergeController;

        private float _currentCooldownTime;
        private bool _cooldown;
        private ITimerModel _timerModel;
        private BottomPanelWindow _bottomPanelWindow;
        private const float IsAvailableCheckInterval = 5f;
        [SerializeField] private CoroutineTimer _spawnTimer;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
           SetTimerAccordingAvailability();
           _coreStateMachine.RunTimeStateMachine.RunTimeState.Subscribe(_ => OnRunTimeStateSwitch(_))
               .AddTo(_disposable);
        }

        private void StartTimer()
        {
            _spawnTimer.InitAndStart((int) _cooldownTime, () =>
            {
                if((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= (int)PlayerUnitTypeEnum.Four )
                    _mergeController.PlaceDefinedItem(((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3));
                else
                    _mergeController.PlaceDefinedItem((int)PlayerUnitTypeEnum.One);
                SetTimerAccordingAvailability();
            }, f => { UpdateFill(f); });

        }

        private void OnRunTimeStateSwitch(RunTimeStateEnum runTimeStateEnum)
        {
            switch (runTimeStateEnum)
            {
                case RunTimeStateEnum.Pause:
                    OnPause();
                    break;
                case RunTimeStateEnum.Play:
                    OnPlay();
                    break;
            }
        }

        private void OnPlay()
        {
            _spawnTimer.StartTick();
        }

        private void OnPause()
        {
            _spawnTimer.Pause();
        }

        private void StartSlotAvailableCheckTimer()
        {
            _spawnTimer.InitAndStart(IsAvailableCheckInterval, () =>
            {
                SetTimerAccordingAvailability();
            });

        }

        private void SetTimerAccordingAvailability()
        {
            _spawnTimer.FinishTimer();
            if (_mergeController.AllSlotsOccupied())
            {
                StartSlotAvailableCheckTimer();
            }
            else
            {
                StartTimer();
            }
        }

        private void UpdateFill(float f)
        {
            if (_bottomPanelWindow != null) _bottomPanelWindow.SetBoxImageFill(1 - (f / _cooldownTime));
        }

        private void OnDisable()
        {
           // _timerModel.StopTick();
           _spawnTimer.FinishTimer();
           _disposable.Clear();
        }
    }
}