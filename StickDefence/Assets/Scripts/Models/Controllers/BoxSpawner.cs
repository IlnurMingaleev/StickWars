using System;
using System.Collections.Generic;
using Enums;
using Models.DataModels;
using Models.Fabrics;
using Models.Merge;
using Models.Timers;
using TonkoGames.Controllers.Core;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Models.Controllers
{
    public class BoxSpawner:MonoBehaviour
    {
        [Inject] private PrefabInject _prefabInject;
        [Inject] private ITimerService _timerService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ConfigManager _configManager;
        private float _cooldownTime =10f;
        

        [Header("MergeController")] [SerializeField]
        private MergeController _mergeController;
        private float _currentCooldownTime;
        private bool _cooldown;
        private ITimerModel _timerModel;
        private BottomPanelWindow _bottomPanelWindow;
        private const float IsAvailableCheckInterval = 5.0f;
        private CompositeDisposable _isAvailableDisposable = new CompositeDisposable();
        private CompositeDisposable _mainTimerDisposable = new CompositeDisposable();
        private float _currentSec;
        [Inject] private IDataCentralService _dataCentralService;
            private void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
           SetTimerAccordingAvailability();
        }

        private void StartTimer()
        {
            _mainTimerDisposable.Clear();
            _currentSec = _cooldownTime; 
            Observable.Timer(TimeSpan.FromMilliseconds(100)).Repeat()
                .Subscribe(_ => TimerSet()).AddTo(_mainTimerDisposable);
            /*_timerModel = _timerService.AddGameTimer(_cooldownTime,
                    f => { UpdateFill(f); },
                    () =>
                    {
                        if((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= (int)PlayerUnitTypeEnum.Four )
                         _mergeController.PlaceDefinedItem(((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3));
                        else
                            _mergeController.PlaceDefinedItem((int)PlayerUnitTypeEnum.One);
                        SetTimerAccordingAvailability();
                    });*/
            
        }

        private void StartSlotAvailableCheckTimer()
        {
            _isAvailableDisposable.Clear();
            Observable.Timer(TimeSpan.FromSeconds(IsAvailableCheckInterval))
                .Subscribe(_ => SetTimerAccordingAvailability()).AddTo(_isAvailableDisposable);
            /*_timerModel = _timerService.AddGameTimer(IsAvailableCheckInterval,
                f => { },
                () => { SetTimerAccordingAvailability(); });*/
        }

        private void SetTimerAccordingAvailability()
        {
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
            _isAvailableDisposable.Clear();
            _mainTimerDisposable.Clear();
            //_timerModel.StopTick();
        }
        private void TimerSet()
        {
            _currentSec -= 0.1f;
            if (_currentSec <= 0)
            {
                _currentSec = 0;
                if((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= (int)PlayerUnitTypeEnum.Four )
                    _mergeController.PlaceDefinedItem(((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3));
                else
                    _mergeController.PlaceDefinedItem((int)PlayerUnitTypeEnum.One);
                _mainTimerDisposable.Clear();
                SetTimerAccordingAvailability();
               
            }
            UpdateFill(_currentSec);
            
        }
    }
}