using System;
using Enums;
using Models.Attacking;
using Models.SO.Core;
using Models.Timers;
using TonkoGames.Sound;
using UniRx;
using UnityEngine;
using Views.Projectiles;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class BasePlayerUnit
    {
        public readonly PlayerView View;
        protected readonly ITimerService TimerService;
        protected readonly ISoundManager SoundManager;

        protected Action<BasePlayerUnit> UnitKilledAction;
        protected Action<ProjectileView> CreateProjectileAction;
        protected Action<ProjectileView> ProjectileDestroyedAction;

        private bool _isPlayable = false;
        private bool _isAttacking = false;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _disposableDead = new CompositeDisposable();
        protected DefaultAttackModel AttackModel;

        protected StickmanStatsConfig UnitStatsConfig;

        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>(false);

      
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;


        public BasePlayerUnit(PlayerView playerView, ITimerService timerService, ISoundManager soundManager) 
        {
            View = playerView;
            TimerService = timerService;
            SoundManager = soundManager;
            playerView.InitUnityActions(OnEnable, OnDisable);
        }

        public void InitActions(Action<BasePlayerUnit> unitKilled)
        {
            UnitKilledAction = unitKilled;
        }

        public virtual void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            CreateProjectileAction = createProjectile;
            ProjectileDestroyedAction = projectileDestroyed;
        }

        public void InitUnitConfigStats(StickmanStatsConfig unitStatsConfig)
        {
            UnitStatsConfig = unitStatsConfig;
            AttackModel.SetDamage(UnitStatsConfig.Damage);
            AttackModel.SetReloading(UnitStatsConfig.Reloading);
        }

        protected virtual void OnEnable()
        {
            View.UnitAnimationCallbacks.AttackAction += AttackActionAnimCallback;
            View.UnitAnimationCallbacks.StartCooldownAttackAction += StartCooldownAttackAnimCallback;
            View.BodyAnimator.speed = 1;
            
            AttackModel.StartPlay();
            
        }

        protected virtual void OnDisable()
        {
            _disposable.Clear();
            View.UnitAnimationCallbacks.AttackAction -= AttackActionAnimCallback;
            View.UnitAnimationCallbacks.StartCooldownAttackAction -= StartCooldownAttackAnimCallback;
            View.BodyAnimator.speed = 0;
            AttackModel.StopPlay();
        }

        public void OnPlay()
        {
            _isPlayable = true;
        }

        public void OnPause()
        {
            _isPlayable = false;
        }
        

        protected virtual void StartAttackAnim()
        {
            _isAttacking = true;
        }

        protected virtual void AttackActionAnimCallback()
        {
            AttackModel.Attack();
        }

        protected virtual void StartCooldownAttackAnimCallback()
        {
            AttackModel.StartCooldown(() => _isAttacking = false);
        }
    }
}