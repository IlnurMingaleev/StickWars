namespace Models.Attacking
{
    public class OneTargetAttack : DefaultAttackModel
    {
        protected override void EndFindAttackTick()
        {
            base.EndFindAttackTick();
            
            TargetDamageable = AttackingCircle.GetNearestEntity(PosAttack.position);
            
            if (TargetDamageable != null)
            {
                IsEnemyFinded = true;
                ClearFindAttackTimer();
                StartAttackAnimAction?.Invoke();
            }
        }
    }
}