using System;
using TonkoGames.Sound;
using Models.Attacking.TypesAttack;
using Models.Merge;
using Models.Player;
using Models.SO.Core;
using Models.Timers;
using UniRx;
using Views.Projectiles;
using Views.Units.Fortress;

namespace Models.Fortress
{
    public class PlayerUnitModel
    {
        public readonly PlayerViewTwo View;
        private readonly IPumping _pumping;
        private readonly ISoundManager _soundManager;
        private readonly ITimerService _timerService;
        private bool _isDead;
        
        private ReactiveProperty<SlotTypeEnum> _parentSlotType = new ReactiveProperty<SlotTypeEnum>(); 
       
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        private RangeOneTargetAttack _rangeAttackModel;
        private CompositeDisposable _disposableIsActive = new CompositeDisposable();
        private StickmanStatsConfig _stickmanStatsConfig;
        private bool _attackSpeedActive = false;
        
        public event Action IsDeadAction;
        public Action<PlayerUnitModel> OnModelRemove;

        #region Getters
        public RangeOneTargetAttack RangeAttackModel => _rangeAttackModel;
        public bool AttackSpeedActive => _attackSpeedActive;
        public IReadOnlyReactiveProperty<SlotTypeEnum> ParentSlotType => _parentSlotType;
        
        #endregion

        #region Setters

        public void SetAttackSpeedActive(bool value) => _attackSpeedActive = value;
        public void SetParentSlotType(SlotTypeEnum slotType)
        {
            _parentSlotType.Value = slotType;
        }

        #endregion
        public PlayerUnitModel(PlayerViewTwo playerView, ISoundManager soundManager, ITimerService timerService,
            StickmanStatsConfig stickmanStatsConfig, bool attackSpeedActive)
        {
            _stickmanStatsConfig = stickmanStatsConfig;
            View = playerView;
            _soundManager = soundManager;
            _timerService = timerService;
            _attackSpeedActive = attackSpeedActive;
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
            _rangeAttackModel = new RangeOneTargetAttack();
            _rangeAttackModel.Init(View.AttackBlockView, _timerService, _soundManager, StartAttackAnim, null);
            _rangeAttackModel.InitProjectileActions(createProjectile, projectileDestroyed);
            _rangeAttackModel.SetProjectile(View.AttackBlockView.ProjectileView);
        }
        
        private void OnEnable()
        {
            SetAttackStats();
           _parentSlotType.Subscribe(slotType =>
            {
                if (slotType == SlotTypeEnum.Attack)
                {
                    _rangeAttackModel.StartCooldown();
                    _rangeAttackModel.StartPlay();
                }
                else
                {
                    _rangeAttackModel.StopPlay();
                }
            }).AddTo(_disposable);
        }

        private void SetAttackStats()
        {
            if (_attackSpeedActive)
            {
                SubscribeStatsWhileAttackSpeedActive();
            }
            else
            {
                SubscribeStats();
            }
        }

        private void OnDisable()
        {
            OnModelRemove?.Invoke(this);
            _disposable.Clear();
            _rangeAttackModel.StopPlay();
        }

        public void SubscribeStats()
        {
            _rangeAttackModel.SetDamage(_stickmanStatsConfig.Damage);
            _rangeAttackModel.SetReloading(_stickmanStatsConfig.Reloading); ;
        }

        public void SubscribeStatsWhileAttackSpeedActive()
        {
            _rangeAttackModel.SetDamage(_stickmanStatsConfig.Damage);
            float reloading = _stickmanStatsConfig.Reloading * 0.8f;
            _rangeAttackModel.SetReloading((int) reloading);
        }
        
        private void OnDead(bool value)
        {
            _isDead = true;
            _rangeAttackModel.Dead();
            IsDeadAction?.Invoke();
        }

        private void StartAttackAnim()
        {
            View.StartLaunchAnim();
            _rangeAttackModel.Attack();
            _rangeAttackModel.StartCooldown();
        }
        
        ~PlayerUnitModel()
        {
            _disposableIsActive.Clear();
        }

        public void OnPause()
        {
           View.OnPause();
           _rangeAttackModel.StopPlay();
        }

        public void OnPlay()
        {
            View.OnPlay();
            _disposable.Clear();
            _parentSlotType.Subscribe(slotType =>
            {
                if (slotType == SlotTypeEnum.Attack)
                {
                    _rangeAttackModel.StartCooldown();
                    _rangeAttackModel.StartPlay();
                }
                else
                {
                    _rangeAttackModel.StopPlay();
                }
            }).AddTo(_disposable);
        }
    }
}