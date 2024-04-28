using Models.Units.UnitType;
using UnityEngine;

namespace Models.Units.Units.Skeleton
{
    public class SkeletonMeleeUnitModel : UnitMeleeOneTargetAttack
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Walk = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");

        protected override void OnDead(bool value)
        {
            base.OnDead(value);
            if (value)
            {
                View.BodyAnimator.SetTrigger(Dead);
                DeadDelay().Forget();
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