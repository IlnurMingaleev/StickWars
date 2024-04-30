using Views.Health;

namespace Models.Attacking
{
    public class SplashTargetAttack : DefaultAttackModel
    {
        protected override bool EndFindAttackTick()
        {
            base.EndFindAttackTick();
            
            if (IsEnemyFinded)
                return false;
            
            var collider2Ds = AttackingCircle.GetAllEntity();

            foreach (var collider2D in collider2Ds)
            {
                if (collider2D.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    SplashDamageables.Add(damageable);
                    IsEnemyFinded = true;
                }
            }

            if (IsEnemyFinded)
            {
                StopCanAttacking();
                StartAttackAnimAction?.Invoke();
            }
            return false;
        }
    }
}