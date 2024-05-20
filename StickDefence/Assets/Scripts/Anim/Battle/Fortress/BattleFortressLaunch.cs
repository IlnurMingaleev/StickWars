 using UnityEngine;
 using UnityEngine.Serialization;

 namespace Anim.Battle.Fortress
{
    public class BattleFortressLaunch : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [FormerlySerializedAs("_fortressCallback")] [SerializeField] private PlayerUnitCallback playerUnitCallback;

        public bool IsLaunchIsProgress { get; private set; }
        public Animator Animator => _animator;
       // private readonly int _launchAnim = Animator.StringToHash("fortress_launch");
        private readonly int Attack = Animator.StringToHash("Attack");
        private readonly int Idle = Animator.StringToHash("Idle");

        private void OnEnable()
        {
            playerUnitCallback.EndAttackAnim += EndAttackAnim;
        }

        private void OnDisable()
        {
            playerUnitCallback.EndAttackAnim -= EndAttackAnim;
        }

        public void StartPrepare()
        {
            _animator.Play(Attack);
        }
        
        public void StartLaunchAnim()
        {
            IsLaunchIsProgress = true;
            _animator.SetTrigger(Attack);
        }

        private void EndAttackAnim()
        {
            IsLaunchIsProgress = false;
            SetDefault();
        }

        private void SetDefault()
        {
            _animator.Play(Idle);
        }
    }
}