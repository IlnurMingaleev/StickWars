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
        private float _cooldownTime =10f;
        

        [Header("MergeController")] [SerializeField]
        private MergeController _mergeController;
        private float _currentCooldownTime;
        private bool _cooldown;
        private ITimerModel _timerModel;
        private BottomPanelWindow _bottomPanelWindow;
        private const float IsAvailableCheckInterval = 5.0f;
        [Inject] private IDataCentralService _dataCentralService;
        private void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
           SetTimerAccordingAvailability();
        }

        private void StartTimer()
        {

            _timerModel = _timerService.AddGameTimer(_cooldownTime,
                    f => { UpdateFill(f); },
                    () =>
                    {
                        if((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value >= (int)PlayerUnitTypeEnum.Four )
                         _mergeController.PlaceDefinedItem(((int)_dataCentralService.PumpingDataModel.MaxStickmanLevel.Value - 3));
                        else
                            _mergeController.PlaceDefinedItem((int)PlayerUnitTypeEnum.One);
                        SetTimerAccordingAvailability();
                    });
            
        }

        private void StartSlotAvailableCheckTimer()
        {

            _timerModel = _timerService.AddGameTimer(IsAvailableCheckInterval,
                f => { },
                () => { SetTimerAccordingAvailability(); });
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
            _timerModel.StopTick();
        }
    }
}