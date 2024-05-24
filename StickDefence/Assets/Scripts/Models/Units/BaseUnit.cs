using System;
using Cysharp.Threading.Tasks;
using Models.Attacking;
using Models.SO.Core;
using Models.Timers;
using TonkoGames.Sound;
using UniRx;
using Views.Projectiles;
using Views.Units.Units;
using Object = UnityEngine.Object;

namespace Models.Units
{
    public class BaseUnit
    {
        protected UnitView View;
        protected ITimerService TimerService;
        protected ISoundManager SoundManager;

        private Action<BaseUnit> UnitKilledAction;
        private bool _isPlayable = false;    
        private bool _isDead;
        private bool IsAttacking => AttackModel.IsEnemyFound;
        
        private readonly CompositeDisposable _disposable = new();
        private readonly CompositeDisposable _disposableDead = new();
        protected DefaultAttackModel AttackModel;

        private UnitStatsConfig _unitStatsConfig;
        private UnitRewardConfig _unitRewardConfig;
        
        private readonly ReactiveProperty<bool> _isMoving = new(false);
      
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;

        public int Experience => _unitRewardConfig.Experience;
        public int Coins => _unitRewardConfig.RewardCount;
        public void InitBase(UnitView unitView, ITimerService timerService, ISoundManager soundManager,
            UnitStatsConfig unitStatsConfig, UnitRewardConfig unitRewardConfig) 
        {
            View = unitView;
            TimerService = timerService;
            SoundManager = soundManager;
            _unitStatsConfig = unitStatsConfig;
            _unitRewardConfig = unitRewardConfig;
            unitView.InitUnityActions(OnEnable, OnDisable);
        }

        public void InitActions(Action<BaseUnit> unitKilled)
        {
            UnitKilledAction = unitKilled;
        }
        
        public virtual void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
        }

        public void InitUnitConfigStats()
        {
            View.Damageable.Init(_unitStatsConfig.Health, _unitStatsConfig.Armor, View.Speed);
            _isMoving.Subscribe(OnWalk).AddTo(_disposable);
            AttackModel.SetDamage(_unitStatsConfig.Damage);
            AttackModel.SetReloading(_unitStatsConfig.Reloading);
        }

        protected virtual void OnEnable()
        {
            View.Damageable.IsEmptyHealth.SkipLatestValueOnSubscribe().Subscribe(OnDead).AddTo(_disposable);
            View.UnitAnimationCallbacks.AttackAction += AttackActionAnimCallback;
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
            View.BodyAnimator.speed = 0;
            AttackModel.StopPlay();
        }

        public void OnPlay()
        {
            _isPlayable = true;
            View.BodyAnimator.enabled = true;
            if (!_isDead)
            {
                AttackModel.StartPlay();
            }
        }

        public void OnPause()
        {
            _isPlayable = false;
            View.BodyAnimator.enabled = false;
            AttackModel.StopPlay();
        }

        public void Update()
        {
            if (!_isPlayable || _isDead || IsAttacking)
            {
                _isMoving.Value = false;
                return;
            }

            _isMoving.Value = true;

            View.UnitFollowPath.Move();
        }

        protected virtual void OnDead(bool value)
        {
            SoundManager.PlayEnemyDeadOneShot();
            _isDead = true;
            AttackModel.Dead();
            View.UnitCollider.enabled = false;
            UnitKilledAction?.Invoke(this);
        }

        protected virtual void OnWalk(bool value)
        {
        }

        protected virtual void StartAttackAnim()
        {
        }

        protected virtual void AttackActionAnimCallback()
        {
        }

        protected async UniTaskVoid DeadDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(View.TimerToDestroy));

            if (View != null)
            {
                Object.Destroy(View.gameObject);
            }
        }
    }
}