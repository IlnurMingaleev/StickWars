using System;
using Enums;
using TonkoGames.Sound;
using Models.Attacking.TypesAttack;
using Models.Merge;
using Models.Player;
using Models.SO.Core;
using Models.Timers;
using Tools.Configs;
using UniRx;
using UnityEditor.Tilemaps;
using UnityEngine;
using Views.Health;
using Views.Projectiles;
using Views.Units.Fortress;
using Views.Units.Units;

namespace Models.Fortress
{
    public class PlayerUnitModel
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        public readonly PlayerViewTwo View;
        private readonly IPumping _pumping;
        private readonly ISoundManager _soundManager;
        private readonly ITimerService _timerService;
        private bool _isDead;
        
        private ReactiveProperty<SlotTypeEnum> _parentSlotType = new ReactiveProperty<SlotTypeEnum>(); 
       
        private bool _isAttacking = false;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private RangeOneTargetAttack _rangeAttackModel;
        private CompositeDisposable _disposableIsActive = new CompositeDisposable();
        private StickmanStatsConfig _stickmanStatsConfig;
        private bool _attackSpeedActive = false;
        
        public event Action IsDeadAction;
        public Action<PlayerUnitModel> OnModelRemove;
        public Action StartAttackAction;

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
            // View.Damageable.InitHealthBar((int)_pumping.GamePerks[PerkTypesEnum.Health].Damage, (int)_pumping.GamePerks[PerkTypesEnum.Defense].Damage);
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
            _rangeAttackModel.Init(View.AttackBlockView, _timerService, _soundManager, StartAttackAnim);
            _rangeAttackModel.SetProjectile(View.AttackBlockView.ProjectileView);
            _rangeAttackModel.InitProjectileActions(createProjectile, projectileDestroyed);
            View.UnitAnimationCallbacks.AttackAction += AttackActionAnimCallback;
            View.UnitAnimationCallbacks.StartCooldownAttackAction += StartCooldownAttackAnimCallback;
        }
       
        private void OnEnable()
        {
          
            View.Animator.speed = 1;
           // View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
           // _pumping.GamePerks.ObserveReplace().Subscribe(_ => SubscribeStats()).AddTo(_disposable);
           SetAttackStats();
           _rangeAttackModel.StartPlay();
            _parentSlotType.Subscribe(slotType =>
            {
                if (slotType == SlotTypeEnum.Attack)
                {
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
            View.UnitAnimationCallbacks.AttackAction -= AttackActionAnimCallback;
            View.UnitAnimationCallbacks.StartCooldownAttackAction -= StartCooldownAttackAnimCallback;
            View.Animator.speed = 0;
            OnModelRemove?.Invoke(this);
            _disposable.Clear();
            _rangeAttackModel.StopPlay();
        }

        public void SubscribeStats()
        {
            _rangeAttackModel.SetDamage(_stickmanStatsConfig.Damage);

            float roundsPerMinute = _stickmanStatsConfig.AttackSpeed;
            float ticks = 60f;
            float reloading = ticks / roundsPerMinute; 
            
            _rangeAttackModel.SetReloading(reloading); ;
        }

        public void SubscribeStatsWhileAttackSpeedActive()
        {
            _rangeAttackModel.SetDamage(_stickmanStatsConfig.Damage);

            float roundsPerMinute = _stickmanStatsConfig.AttackSpeed * 1.5f;
            float ticks = 60f;
            float reloading = ticks / roundsPerMinute; 
            
            _rangeAttackModel.SetReloading(reloading);
            
        }
        
        private void OnDead(bool value)
        {
            _isDead = true;
            _rangeAttackModel.StopPlay();
            IsDeadAction?.Invoke();
        }
        
        
        
        ~PlayerUnitModel()
        {
            _disposableIsActive.Clear();
        }
        protected virtual void StartAttackAnim()
        {
            _isAttacking = true;
        }
        public void OnPlay()
        {
            
            _rangeAttackModel.StartPlay();
        }

        public void OnPause()
        {
            _rangeAttackModel.StopPlay();
        }
        protected virtual void AttackActionAnimCallback()
        {
            _rangeAttackModel.Attack();
        }

        protected virtual void StartCooldownAttackAnimCallback()
        {
            _rangeAttackModel.StartCooldown(() => _isAttacking = false);
        }
    }
}