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
using UnityEngine;
using Views.Health;
using Views.Projectiles;
using Views.Units.Fortress;
using Views.Units.Units;

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
        public event Action IsDeadAction;

        #region Getters
        public IReadOnlyReactiveProperty<SlotTypeEnum> ParentSlotType => _parentSlotType;
        
        #endregion

        #region Setters

        public void SetParentSlotType(SlotTypeEnum slotType)
        {
            _parentSlotType.Value = slotType;
        }

        #endregion
        public PlayerUnitModel(PlayerViewTwo playerView, ISoundManager soundManager, ITimerService timerService,StickmanStatsConfig stickmanStatsConfig)
        {
            _stickmanStatsConfig = stickmanStatsConfig;
            View = playerView;
            _soundManager = soundManager;
            _timerService = timerService;
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
            _rangeAttackModel.InitProjectileActions(createProjectile, projectileDestroyed);
            _rangeAttackModel.SetProjectile(View.AttackBlockView.ProjectileView);
        }
        
        private void OnEnable()
        {
           
           // View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
           // _pumping.GamePerks.ObserveReplace().Subscribe(_ => SubscribeStats()).AddTo(_disposable);
            SubscribeStats();
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

        private void OnDisable()
        {
            _disposable.Clear();
            _rangeAttackModel.StopPlay();
        }

        private void SubscribeStats()
        {
            _rangeAttackModel.SetDamage(_stickmanStatsConfig.Damage);

            float roundsPerMinute = _stickmanStatsConfig.AttackSpeed;
            float ticks = 60f;
            float reloading = ticks / roundsPerMinute; 
            
            _rangeAttackModel.SetReloading(reloading);

           // View.Damageable.SetMaxHealth((int)_pumping.GamePerks[PerkTypesEnum.Health].Damage);
           // View.Damageable.UpdateDefence(_pumping.GamePerks[PerkTypesEnum.Defense].Damage);
            
           // _rangeAttackModel.ReSetupRangeAttack(_pumping.GamePerks[PerkTypesEnum.AttackRange].Damage);
        }

        private void OnDead(bool value)
        {
            _isDead = true;
            _rangeAttackModel.StopPlay();
            IsDeadAction?.Invoke();
        }

        private void StartAttackAnim()
        {
            _rangeAttackModel.Attack();
            _rangeAttackModel.StartCooldown(null);
        }
        
        ~PlayerUnitModel()
        {
            _disposableIsActive.Clear();
        }
    }
}