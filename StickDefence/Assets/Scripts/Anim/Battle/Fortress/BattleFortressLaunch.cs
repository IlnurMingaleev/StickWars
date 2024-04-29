 using UnityEngine;

 namespace Anim.Battle.Fortress
{
    public class BattleFortressLaunch : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private FortressCallback _fortressCallback;

        public bool IsLaunchIsProgress { get; private set; }
        
       // private readonly int _launchAnim = Animator.StringToHash("fortress_launch");
        private readonly int Attack = Animator.StringToHash("Attack");
        private readonly int Idle = Animator.StringToHash("Idle");

        private void OnEnable()
        {
            _fortressCallback.EndLaunchAnim += EndLaunchAnim;
        }

        private void OnDisable()
        {
            _fortressCallback.EndLaunchAnim -= EndLaunchAnim;
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

        private void EndLaunchAnim()
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