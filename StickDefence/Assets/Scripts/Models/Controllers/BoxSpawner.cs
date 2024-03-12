using System;
using System.Collections.Generic;
using Enums;
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
        [Inject] private ITimerService _timerService;
        [Inject] private IWindowManager _windowManager;
        [Inject] private ConfigManager _configManager;
        [SerializeField] private float _cooldownTime;
        private float _currentCooldownTime;
        private bool _cooldown;
        public List<Transform> _listOfAvailableTiles = new List<Transform>();
        private ITimerModel _timerModel;
        private BattleWindow _battleWindow;
        private void Start()
        {
            _battleWindow = _windowManager.GetWindow<BattleWindow>();
            StartTimer();
        }

        private void StartTimer()
        {
            if (_listOfAvailableTiles.Count == 0) return;
            
            _timerModel = _timerService.AddGameTimer(_cooldownTime,
                    f => { UpdateFill(f); },
                    () =>
                    {
                        Transform tileToSpawn = _listOfAvailableTiles[Random.Range(0, _listOfAvailableTiles.Count - 1)];
                        GameObject _currentPlayer = Instantiate(
                            _configManager.PrefabsUnitsSO.PlayerUnitPrefabs[PlayerUnitTypeEnum.PlayerOne],
                            tileToSpawn.position, tileToSpawn.rotation);
                        _listOfAvailableTiles.Remove(tileToSpawn);
                        StartTimer();

                    });
            
        }

        private void UpdateFill(float f)
        {
            if (_battleWindow != null) _battleWindow.SetImageFill(1 - (f / _cooldownTime));
        }
    }
}