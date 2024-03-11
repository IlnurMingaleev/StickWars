using UnityEngine;
using Views.Health;

namespace Views.Projectiles
{
    public class ProjectileOneTarget : ProjectileView
    {
        protected override void OnHit(Collider2D other)
        {
            if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.SetDamage(Damage);
                InstantiateParticle(_particlePrefabHit);
                PlayHitTarget();
            }
            else
            {
                InstantiateParticle(_particlePrefabDead);
            }
        }
    }
}