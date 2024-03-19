using System;
using Models.Attacking.TypesAttack;
using Models.Timers;
using TonkoGames.Sound;
using UnityEngine;
using Views.Projectiles;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class SkeletonUnitModel : BaseUnit
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        public SkeletonUnitModel(UnitView unitView, ITimerService timerService, ISoundManager soundManager) : base(unitView, timerService, soundManager)
        {
        }
        
        public override void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            AttackModel = new RangeOneTargetAttack();
            AttackModel.Init(View.AttackBlockView, TimerService, SoundManager, StartAttackAnim);
          
            base.InitAttack(createProjectile, projectileDestroyed);
        }

        protected override void OnDead(bool value)
        {
            base.OnDead(value);
            if (value)
            {
                View.BodyAnimator.SetTrigger(Dead);
                TimerService.AddGameTimer(View.TimerToDestroy, null, () =>
                {
                    if (View != null)
                    {
                        UnitKilledAction?.Invoke(this);
                    }
                });
            }
        }

        protected override void OnWalk(bool value)
        {
            base.OnWalk(value);
            View.BodyAnimator.SetBool(Walk, value);
        }

        protected override void StartAttackAnim()
        {
            base.StartAttackAnim();
            View.BodyAnimator.SetTrigger(Attack);
        }
    }
}