using System;
using Models.Attacking.TypesAttack;
using Models.Merge;
using Models.SO.Core;
using Models.Timers;
using TonkoGames.Sound;
using UniRx;
using Views.Projectiles;
using Views.Units.Units;
using Object = UnityEngine.Object;

namespace Models.Units.Units
{
    public class BasePlayerUnit: IDisposable
    {
        protected readonly PlayerView View;
        protected readonly ITimerService TimerService;
        protected readonly ISoundManager SoundManager;

        protected Action<BasePlayerUnit> UnitKilledAction;

        private bool _isPlayable = false;
        private bool IsAttacking => AttackModel.IsEnemyFound;

        private readonly CompositeDisposable _disposable = new();
        private readonly CompositeDisposable _disposableDead = new();
        protected RangeOneTargetAttack AttackModel;

        private readonly ReactiveProperty<bool> _isMoving = new(false);
        private readonly ReactiveProperty<SlotTypeEnum> _parentSlotType = new();
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
        }

        public void InitUnitConfigStats(StickmanStatsConfig unitStatsConfig)
        {
            StickmanStatsConfig stickManStatsConfig = unitStatsConfig;
            AttackModel.SetDamage(stickManStatsConfig.Damage);
            AttackModel.SetReloading(stickManStatsConfig.Reloading);
        }

        protected virtual void OnEnable()
        {
            View.UnitAnimationCallbacks.AttackAction += AttackActionAnimCallback;
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
        }

        protected virtual void AttackActionAnimCallback()
        {
            AttackModel.Attack();
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