using UnityEngine;
using Views.Projectiles;

namespace Models.Attacking
{
    public class AttackBlockView : MonoBehaviour
    {
        [field: SerializeField] public Transform PosAttack            { get; private set; }
        [field: SerializeField] public Transform PosSpawnProjectile   { get; private set; }
        [field: SerializeField] public float AttackRange              { get; private set; }
        [field: SerializeField] public ContactFilter2D ContactFilter  { get; private set; }
        [field: SerializeField] public ProjectileView ProjectileView  { get; private set; }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PosAttack.position, AttackRange);
        }
#endif
    }
}