using System;
using Models.Attacking.TypesAttack;
using Models.Timers;
using TonkoGames.Sound;
using UnityEngine;
using Views.Projectiles;
using Views.Units.Units;

namespace Models.Units.Units
{
    public class BasePlayerUnitUnitOne: BasePlayerUnit
    {
        private static readonly int Dead = Animator.StringToHash("Dead");
        private static readonly int Attack = Animator.StringToHash("Attack");
        public BasePlayerUnitUnitOne(PlayerView playerView, ITimerService timerService, ISoundManager soundManager) : base(playerView, timerService, soundManager)
        {
        }
        
        public override void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            AttackModel = new RangeOneTargetAttack();
            AttackModel.Init(View.AttackBlockView, TimerService, SoundManager, StartAttackAnim);
            AttackModel.InitProjectileActions(createProjectile, projectileDestroyed);
            AttackModel.SetProjectile(View.AttackBlockView.ProjectileView);
        }
        protected override void StartAttackAnim()
        {
            base.StartAttackAnim();
            View.BodyAnimator.SetTrigger(Attack);
        }
    }
}