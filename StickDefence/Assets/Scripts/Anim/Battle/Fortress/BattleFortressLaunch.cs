 using UnityEngine;

 namespace Anim.Battle.Fortress
{
    public class BattleFortressLaunch : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private FortressCallback _fortressCallback;

        public bool IsLaunchIsProgress { get; private set; }
        
        private readonly int _launchAnim = Animator.StringToHash("fortress_launch");
        private readonly int _prepareAnim = Animator.StringToHash("fortress_prepare_launch");
        private readonly int _defaultAnim = Animator.StringToHash("fortress_default_state");

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
            _animator.Play(_prepareAnim);
        }
        
        public void StartLaunchAnim()
        {
            IsLaunchIsProgress = true;
            _animator.StopPlayback();
            _animator.Play(_launchAnim);
        }

        private void EndLaunchAnim()
        {
            IsLaunchIsProgress = false;
            SetDefault();
        }

        private void SetDefault()
        {
            _animator.Play(_defaultAnim);
            _animator.StopPlayback();
        }
    }
}