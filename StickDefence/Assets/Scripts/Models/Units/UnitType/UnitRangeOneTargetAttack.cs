using System;
using Models.Attacking.TypesAttack;
using UnityEngine;
using Views.Projectiles;

namespace Models.Units.UnitType
{
    public class UnitRangeOneTargetAttack : BaseUnit
    {
        public override void InitAttack(Action<ProjectileView> createProjectile,
            Action<ProjectileView> projectileDestroyed)
        {
            RangeOneTargetAttack tmpAttack = new RangeOneTargetAttack();
            
            tmpAttack.SetProjectile(View.AttackBlockView.ProjectileView);
            tmpAttack.InitProjectileActions(createProjectile, projectileDestroyed);
            AttackModel = tmpAttack;
            AttackModel.Init(View.AttackBlockView,TimerService,SoundManager,StartAttackAnim,null);
            base.InitAttack(createProjectile, projectileDestroyed);
        }
    }
}