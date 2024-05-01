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
                IsEnemyFound = true;
                StartAttackAnim();
                Attack();
                return;
            }
            
            IsEnemyFound = false;
        }
    }
}