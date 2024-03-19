using System;
using Models.Attacking;
using Models.Attacking.TypesAttack;
using Models.SO.Core;
using Models.Timers;
using TonkoGames.Sound;
using UniRx;
using Views.Projectiles;
using Views.Units.Units;

namespace Models.Units
{
    public class BaseUnit
    {
        public readonly UnitView View;
        protected readonly ITimerService TimerService;
        protected readonly ISoundManager SoundManager;

        protected Action<BaseUnit> UnitKilledAction;
        protected Action<ProjectileView> CreateProjectileAction;
        protected Action<ProjectileView> ProjectileDestroyedAction;

        private bool _isPlayable = false;    
        private bool _isDead;
        private bool _isAttacking = false;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private CompositeDisposable _disposableDead = new CompositeDisposable();
        protected DefaultAttackModel AttackModel;
        
        protected UnitStatsConfig UnitStatsConfig;
        
        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>(false);
        
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
        
        public BaseUnit(UnitView unitView, ITimerService timerService, ISoundManager soundManager) 
        {
            View = unitView;
            TimerService = timerService;
            SoundManager = soundManager;
            unitView.InitUnityActions(OnEnable, OnDisable);
        }

        public void InitActions(Action<BaseUnit> unitKilled)
        {
            UnitKilledAction = unitKilled;
        }
        
        public virtual void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            CreateProjectileAction = createProjectile;
            ProjectileDestroyedAction = projectileDestroyed;
            ((RangeOneTargetAttack)(AttackModel)).SetProjectile(View.AttackBlockView.ProjectileView);
        }

        public void InitUnitConfigStats(UnitStatsConfig unitStatsConfig)
        {
            UnitStatsConfig = unitStatsConfig;
            View.Damageable.Init(UnitStatsConfig.Health, UnitStatsConfig.Armor);
            _isMoving.Subscribe(OnWalk).AddTo(_disposable);
            AttackModel.SetDamage(UnitStatsConfig.Damage);
            AttackModel.SetReloading(UnitStatsConfig.Reloading);
           
        }

        protected virtual void OnEnable()
        {
            View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
            View.UnitAnimationCallbacks.AttackAction += AttackActionAnimCallback;
            View.UnitAnimationCallbacks.StartCooldownAttackAction += StartCooldownAttackAnimCallback;
            View.BodyAnimator.speed = 1;

            if (!_isDead)
            {
                AttackModel.StartPlay();
            }
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

        public void Update()
        {
            if (!_isPlayable || _isDead || _isAttacking)
            {
                _isMoving.Value = false;
                return;
            }

            _isMoving.Value = true;

            View.UnitFollowPath.Move();
        }

        protected virtual void OnDead(bool value)
        {
            _isDead = true;
            AttackModel.StopPlay();
            View.UnitCollider.enabled = false;
        }

        protected virtual void OnWalk(bool value)
        {
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