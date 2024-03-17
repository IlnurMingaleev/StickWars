using System;
using System.Collections.Generic;
using Enums;
using Models.Fabrics;
using Models.Merge;
using Models.Timers;
using TonkoGames.Controllers.Core;
using UI.UIManager;
using UI.Windows;
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
        [SerializeField] private float _cooldownTime;

        [Header("MergeController")] [SerializeField]
        private MergeController _mergeController;
        private float _currentCooldownTime;
        private bool _cooldown;
        private ITimerModel _timerModel;
        private BottomPanelWindow _bottomPanelWindow;
        private void Start()
        {
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
            StartTimer();
        }

        private void StartTimer()
        {

            _timerModel = _timerService.AddGameTimer(_cooldownTime,
                    f => { UpdateFill(f); },
                    () =>
                    {
                         _mergeController.PlaceDefinedItem((int)PlayerUnitTypeEnum.PlayerOne);
                         StartTimer();

                    });
            
        }

        private void UpdateFill(float f)
        {
            if (_bottomPanelWindow != null) _bottomPanelWindow.SetBoxImageFill(1 - (f / _cooldownTime));
        }
    }
}