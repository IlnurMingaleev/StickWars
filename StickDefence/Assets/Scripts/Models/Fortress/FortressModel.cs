using System;
using Enums;
using TonkoGames.Sound;
using Models.Attacking.TypesAttack;
using Models.Player;
using Models.Timers;
using UniRx;
using UnityEngine;
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
        private bool _isDead;
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        private RangeOneTargetAttack _rangeAttackModel;
        private CompositeDisposable _disposableIsActive = new CompositeDisposable();
        public event Action IsDeadAction;
        
        public FortressModel(FortressView fortressView, ISoundManager soundManager, ITimerService timerService, IPumping pumping)
        {
            _pumping = pumping;
            View = fortressView;
            _soundManager = soundManager;
            _timerService = timerService;
            View.Damageable.Init(_pumping.WallData[WallTypeEnum.Basic].HealthValue, (int)_pumping.GamePerks[PerkTypesEnum.Defense].Value);
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
            View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
            _pumping.GamePerks.ObserveReplace().Subscribe(_ => SubscribeStats()).AddTo(_disposable);
            SubscribeStats();
            if (!_isDead)
            {
                _rangeAttackModel.StartPlay();
            }
        }

        private void OnDisable()
        {
            _disposable.Clear();
            _rangeAttackModel.StopPlay();
        }

        private void SubscribeStats()
        {
            _rangeAttackModel.SetDamage((int) _pumping.GamePerks[PerkTypesEnum.Damage].Value);

            float roundsPerMinute = _pumping.GamePerks[PerkTypesEnum.AttackSpeed].Value;
            float ticks = 60f;
            float reloading = ticks / roundsPerMinute; 
            
            _rangeAttackModel.SetReloading(reloading);

            View.Damageable.SetMaxHealth((int)_pumping.WallData[WallTypeEnum.Basic].HealthValue);
            View.Damageable.UpdateDefence(_pumping.GamePerks[PerkTypesEnum.Defense].Value);
            
           // _rangeAttackModel.ReSetupRangeAttack(_pumping.GamePerks[PerkTypesEnum.AttackRange].Value);
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
        
        ~FortressModel()
        {
            _disposableIsActive.Clear();
        }
    }
}