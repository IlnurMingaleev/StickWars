namespace Models.Attacking.TypesAttack
{
    public class MeleeOneTargetAttack : OneTargetAttack
    {
        public override void Attack()
        {
            base.Attack();

            TargetDamageable?.SetDamage(Damage);
        }
    }
}