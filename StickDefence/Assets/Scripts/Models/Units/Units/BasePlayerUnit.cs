using System;
using Enums;
using Models.Attacking;
using Models.Attacking.TypesAttack;
using Models.Merge;
using Models.SO.Core;
using Models.Timers;
using TonkoGames.Sound;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Views.Projectiles;
using Views.Units.Units;
using Object = UnityEngine.Object;

namespace Models.Units.Units
{
    public class BasePlayerUnit: IDisposable
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
        protected RangeOneTargetAttack AttackModel;

        protected StickmanStatsConfig UnitStatsConfig;

        private ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>(false);
        private ReactiveProperty<SlotTypeEnum> _parentSlotType = new ReactiveProperty<SlotTypeEnum>();
        public IReadOnlyReactiveProperty<SlotTypeEnum> ParentSlotType => _parentSlotType;
      
        public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;


        public BasePlayerUnit(PlayerView playerView, ITimerService timerService, ISoundManager soundManager) 
        {
            View = playerView;
            TimerService = timerService;
            SoundManager = soundManager;
            playerView.InitUnityActions(OnEnable, OnDisable);
        }
        public void InitParentSlotType(SlotTypeEnum slotType)
        {
            _parentSlotType.Value = slotType;
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
            /*_parentSlotType.Subscribe(_ =>
            {
                AttackModel.StartPlay();
                    
              
                
                    AttackModel.StopPlay();
                
            }).AddTo(_disposable);*/


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

        public void Dispose()
        {
            _disposable.Clear();
            _disposableDead.Clear();
            if (View != null)
            {
                Object.Destroy(View);
            }
        }
    }
}