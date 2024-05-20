using System.Collections.Generic;
using Models.DataModels;
using TonkoGames.Controllers.Core;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.Fortress;
using Models.Player;
using Models.Timers;
using UI.UIManager;
using UnityEngine;
using VContainer;
using Views.Projectiles;
using Views.Units.Fortress;

namespace Models.Battle
{
    public class PlayerFortressInstantiate : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private FortressView _fortressView;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly ConfigManager _configManager;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private readonly ITimerService _timerService;
        [Inject] private readonly IPlayer _player;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly IDataCentralService _dataCentralService;
        
        private int _maxHealth = 1;
        private List<ProjectileView> _projectiles = new();
        
        private FortressModel _fortressModel;

        private void OnEnable()
        {
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.SubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
        }

        private void OnDisable()
        {
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Play, OnPlayRunTimes);
            _coreStateMachine.RunTimeStateMachine.UnSubscriptionAction(RunTimeStateEnum.Pause, OnPauseRunTime);
            DestroyStage();
        }
        
        public void InitStageLoadBattle()
        {
            InitFortress();
        }
        
        public void Resurrect()
        {
            _fortressModel.Resurrect();
        }
        
        public float GetDeltaHealth() => (float)_fortressView.Damageable.HealthCurrent.Value / (float)_fortressView.Damageable.HealthMax.Value;

        public void DestroyStage()
        {
           
            foreach (var projectileView in _projectiles)
            {
                Destroy(projectileView.gameObject);
            }
            _projectiles.Clear();
        }

        private void InitFortress()
        {
            _fortressModel = new FortressModel(_fortressView, _soundManager, _timerService,
                _player.Pumping, _windowManager,_coreStateMachine,_dataCentralService);
            _fortressModel.InitBottomPanelButton();
            _fortressModel.InitSubActive();

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
            foreach (var projectileView in _projectiles)
            {
                projectileView.StartMove();
            }
        }

        private void OnPauseRunTime()
        {
            foreach (var projectileView in _projectiles)
            {
                projectileView.StopMove();
            }  
        }
    }
}