namespace Models.Attacking
{
    public class OneTargetAttack : DefaultAttackModel
    {
        protected override bool EndFindAttackTick()
        {
            base.EndFindAttackTick();
            
            TargetDamageable = AttackingCircle.GetNearestEntity(PosAttack.position);
            
            if (TargetDamageable != null)
            {
                IsEnemyFinded = true;
                StopCanAttacking();
                StartAttackAnimAction?.Invoke();
                return true;
            }
            
            return false;
        }
    }
}