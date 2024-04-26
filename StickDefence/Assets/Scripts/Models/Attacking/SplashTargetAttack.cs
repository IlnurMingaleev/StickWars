using Views.Health;

namespace Models.Attacking
{
    public class SplashTargetAttack : DefaultAttackModel
    {
        protected override void EndFindAttackTick()
        {
            base.EndFindAttackTick();
            
            if (IsEnemyFound)
                return;
            
            var collider2Ds = AttackingCircle.GetAllEntity();

            foreach (var collider2D in collider2Ds)
            {
                if (collider2D.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    SplashDamageables.Add(damageable);
                    IsEnemyFound = true;
                }
            }

            if (IsEnemyFound)
            {
                StartAttackAnim();
                return;
            }
            
            IsEnemyFound = false;
        }
    }
}