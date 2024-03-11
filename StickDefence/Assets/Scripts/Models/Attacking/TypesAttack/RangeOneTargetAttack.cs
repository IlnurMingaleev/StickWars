using System;
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

            if (TargetDamageable != null)
            {
                var projectile = Object.Instantiate(_projectileViewPrefab, PosAttack.position, Quaternion.identity);
                var deadTimer = TimerService.AddGameTimer(projectile.DeathTime, null, () =>
                {
                    Object.Destroy(projectile.gameObject);
                });
                projectile.Init(Damage, TargetDamageable.GetTransform().position, ContactFilter.layerMask, SoundManager, _projectileDestroyed, deadTimer);
                _createProjectile?.Invoke(projectile);
            }
        }
    }
}