using Tools.GameTools;
using UnityEngine;

namespace AnimationsTools.AnimCallbacks
{
    public class AnimShowTool : TransitAnimCallback
    {
        [SerializeField] private Animator _animator;
        
        public void ShowAnim()
        {
            //_animator.enabled = true;
            _animator.SetTrigger(StringsHelper.Helper.Start);
        }

        public override void OnTransitionCallback()
        {
            base.OnTransitionCallback();
            //_animator.enabled = false;
        }
    }
}