namespace Models.Attacking
{
    public class OneTargetAttack : DefaultAttackModel
    {
        protected override void EndFindAttackTick()
        {
            base.EndFindAttackTick();
            if(TargetDamageable == null) TargetDamageable = AttackingCircle.GetNearestEntity(PosAttack.position);
            else if(TargetDamageable.Dead) TargetDamageable = AttackingCircle.GetNearestEntity(PosAttack.position);
            
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