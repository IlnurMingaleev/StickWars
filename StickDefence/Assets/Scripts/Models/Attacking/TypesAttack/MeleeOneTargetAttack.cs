namespace Models.Attacking.TypesAttack
{
    public class MeleeOneTargetAttack : OneTargetAttack
    {
        public override void Attack()
        {
           
            base.Attack();
            if (TargetDamageable != null && PosAttack == null)
            {
                return;
            }
            
            SetDamage(TargetDamageable);
           
        }
    }
}