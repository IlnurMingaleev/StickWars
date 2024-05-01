using System;
using Tools.Extensions;
using UnityEngine;
using Views.Projectiles;
using Object = UnityEngine.Object;

namespace Models.Attacking.TypesAttack
{
    public class RangeOneTargetAttack : OneTargetAttack
    {
        private ProjectileView _projectileViewPrefab;
        private Action<ProjectileView> _createProjectile;
        private Action<ProjectileView> _projectileDestroyed;

        public void InitProjectileActions(Action<ProjectileView> createProjectile, Action<ProjectileView> projectileDestroyed)
        {
            _createProjectile = createProjectile;
            _projectileDestroyed = projectileDestroyed;
        }
        
        public void SetProjectile(ProjectileView prefab)
        {
            _projectileViewPrefab = prefab;
        }

        public override void Attack()
        {
            base.Attack();
            if (TargetDamageable != null && PosAttack != null)
            {
                var projectile = Object.Instantiate(_projectileViewPrefab, PosAttack.position, Quaternion.identity);

                var isCritical = IsCritical();

                var predictCollisionPoint = projectile.transform.position.CalculatePredictCollision(projectile.Speed, 
                    TargetDamageable.GetTransformCenterPoint().position,
                    TargetDamageable.SpeedToCalculatePredict.Value);
                projectile.Init(DamageCritical(isCritical),predictCollisionPoint, isCritical, ContactFilter.layerMask, SoundManager, _projectileDestroyed, TimerService);
                _createProjectile?.Invoke(projectile);
            }
           
        }
    }
}