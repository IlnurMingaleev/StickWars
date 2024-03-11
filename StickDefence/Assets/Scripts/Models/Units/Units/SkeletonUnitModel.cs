using System;
using UnityEngine;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class SkeletonUnitModel : BaseUnit
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        public SkeletonUnitModel(UnitView unitView) : base(unitView)
        {
        }

        protected override void OnDead(bool value)
        {
            base.OnDead(value);
            if (value)
            {
                View.BodyAnimator.SetTrigger(Dead);
            }
        }

        protected override void OnWalk(bool value)
        {
            View.BodyAnimator.SetBool(Walk, value);
        }
    }
}