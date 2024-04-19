using System;
using Models.Attacking.TypesAttack;
using Views.Projectiles;

namespace Models.Units.UnitType
{
    public class UnitRangeOneTargetAttack : BaseUnit
    {
        public override void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            RangeOneTargetAttack tmpAttack = new RangeOneTargetAttack();
            
            AttackModel = tmpAttack;
            base.InitAttack(createProjectile, projectileDestroyed);
            
            tmpAttack.Init(View.AttackBlockView, TimerService, SoundManager, StartAttackAnim);
            
            tmpAttack.InitProjectileActions(createProjectile, projectileDestroyed);
            tmpAttack.SetProjectile(View.AttackBlockView.ProjectileView);
        }
    }
}