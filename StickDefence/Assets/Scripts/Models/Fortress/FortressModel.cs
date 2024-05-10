using System;
using Enums;
using Models.DataModels;
using TonkoGames.Sound;
using Models.Player;
using Models.Timers;
using TonkoGames.StateMachine;
using UI.UIManager;
using UI.Windows;
using UniRx;
using Views.Projectiles;
using Views.Units.Fortress;

namespace Models.Fortress
{
    public class FortressModel
    {
        public readonly FortressView View;
        private readonly IPumping _pumping;
        private readonly ISoundManager _soundManager;
        private readonly ITimerService _timerService;
        private readonly IWindowManager _windowManager;
        private readonly ICoreStateMachine _coreStateMachine;
        private readonly IDataCentralService _dataCentralService;
        private BottomPanelWindow _bottomPanelWindow;
        private bool _isDead;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _disposableIsActive = new CompositeDisposable();
        public event Action IsDeadAction;
        
        public FortressModel(FortressView fortressView, ISoundManager soundManager,
            ITimerService timerService, IPumping pumping, IWindowManager windowManager,
            ICoreStateMachine coreStateMachine, IDataCentralService dataCentralService)
        {
            _coreStateMachine = coreStateMachine;
            _windowManager = windowManager;
            _pumping = pumping;
            View = fortressView;
            _soundManager = soundManager;
            _timerService = timerService;
            _dataCentralService = dataCentralService;
            View.Damageable.Init(_pumping.WallData[WallTypeEnum.Basic].HealthValue, _pumping.WallData[WallTypeEnum.Basic].Defense);
            _bottomPanelWindow = _windowManager.GetWindow<BottomPanelWindow>();
        }

        public void InitBottomPanelButton()
        {
            InitHealthBar();
            InitBottomPanelEvent();
            SubscribeToOnClickEvent();
        }

        public void InitHealthBar()
        {
            View.SetLevelLabel(_dataCentralService.PumpingDataModel.WallLevelReactive.Value.WallLevel);
            View.Damageable.HealthCurrent.Subscribe(health =>UpdateWallHealthBar(health) ).AddTo(_disposable);
            View.Damageable.HealthMax.Subscribe(healthMax => UpdateWallHealthMax(healthMax)).AddTo(_disposable);
        }

        public void InitBottomPanelEvent()
        {
            if (_bottomPanelWindow)
            {
                _bottomPanelWindow.UpdateWallCost(_pumping.WallData[WallTypeEnum.Basic].Cost);
            }
            _pumping.WallPumpingEvents.InitEvents((cost) =>
            {
                if(_bottomPanelWindow) _bottomPanelWindow.UpdateWallCost(cost);
                SubscribeStats();
            }, health =>
            {
                if(_bottomPanelWindow) UpdateWallHealthMax(health);
                SubscribeStats();
            });
        }

        public void SubscribeToOnClickEvent()
        {
            if(_bottomPanelWindow) _bottomPanelWindow.UpgradeWallClickedEvent += UpgradeWall;
        }
        public void UnsubscribeToOnClickEvent()
        {
            if(_bottomPanelWindow) _bottomPanelWindow.UpgradeWallClickedEvent -= UpgradeWall;
        }
        private void UpgradeWall()
        {
            _pumping.UpgradeWall(WallTypeEnum.Basic);
            View.SetLevelLabel(_dataCentralService.PumpingDataModel.WallLevelReactive.Value.WallLevel);
        }

        private void UpdateWallHealthBar(int health)
        {
            if(_bottomPanelWindow)
                _bottomPanelWindow.UpdateWallHealthBar(health, View.Damageable.HealthMax.Value);
        }
        
        private void UpdateWallHealthMax(int healthMax)
        {
            if(_bottomPanelWindow)
                _bottomPanelWindow.UpdateWallHealthBar( healthMax,healthMax);
        }
        public void InitSubActive()
        {
            View.IsActive.Subscribe(value =>
            {
                if (value)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }).AddTo(_disposableIsActive);
        }
        
        public void InitAttack(Action<ProjectileView> createProjectile, Action<ProjectileView> projectileDestroyed)
        {
           // _rangeAttackModel = new RangeOneTargetAttack();
            //_rangeAttackModel.Init(View.AttackBlockView, _timerService, _soundManager, StartAttackAnim);
           // _rangeAttackModel.InitProjectileActions(createProjectile, projectileDestroyed);
           // _rangeAttackModel.SetProjectile(View.AttackBlockView.ProjectileView);
        }
        
        private void OnEnable()
        {
            View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
            _pumping.GamePerks.ObserveReplace().Subscribe(_ => SubscribeStats()).AddTo(_disposable);
            SubscribeStats();
            if (!_isDead)
            {
                //_rangeAttackModel.StartPlay();
            }
        }

        private void OnDisable()
        {
            _disposable.Clear();
            //_rangeAttackModel.StopPlay();
            UnsubscribeToOnClickEvent();
        }

        private void SubscribeStats()
        {
            View.Damageable.SetMaxHealth((int)_pumping.WallData[WallTypeEnum.Basic].HealthValue);
            View.Damageable.UpdateDefence(_pumping.WallData[WallTypeEnum.Basic].Defense);
            View.Damageable.Resurrect();
        }

        private void OnDead(bool value)
        {
            _isDead = true;
            //_rangeAttackModel.Dead();
            _coreStateMachine.BattleStateMachine.OnEndBattle(false);
            IsDeadAction?.Invoke();
        }

        private void StartAttackAnim()
        {
           // _rangeAttackModel.Attack();
           // _rangeAttackModel.StartCooldown(null);
        }

        public void Resurrect()
        {
            View.Damageable.Resurrect();
        }

        ~FortressModel()
        {
            _disposableIsActive.Clear();
        }
    }
}