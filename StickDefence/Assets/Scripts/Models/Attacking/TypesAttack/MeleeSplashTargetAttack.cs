namespace Models.Attacking.TypesAttack
{
    public class MeleeSplashTargetAttack : SplashTargetAttack
    {
        public override void Attack()
        {
            base.Attack();

            if (TargetDamageable != null && PosAttack == null)
            {
                return;
            }
            
            foreach (var damageable in SplashDamageables)
            {
                SetDamage(damageable);
            }
            
            SplashDamageables.Clear();
        }
    }
}