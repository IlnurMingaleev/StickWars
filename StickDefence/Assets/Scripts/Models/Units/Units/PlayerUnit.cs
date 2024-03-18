using Enums;
using Models.Timers;
using TonkoGames.Sound;
using UnityEngine;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class PlayerUnit : BaseUnit
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Attack = Animator.StringToHash("Attack");
        public PlayerUnit(UnitView unitView, ITimerService timerService, ISoundManager soundManager) : base(unitView, timerService, soundManager)
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
        
        
    }
}