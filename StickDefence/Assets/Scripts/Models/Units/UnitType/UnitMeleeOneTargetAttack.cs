using System;
using Models.Attacking.TypesAttack;
using Views.Projectiles;

namespace Models.Units.UnitType
{
    public class UnitMeleeOneTargetAttack : BaseUnit
    {
        public override void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            AttackModel = new MeleeOneTargetAttack();
            AttackModel.Init(View.AttackBlockView,TimerService,SoundManager,StartAttackAnim,null);
        }
    }
}