using System.Collections.Generic;
using DG.Tweening;
using UI.UIManager;
using UniRx;

namespace UI.Content.TopPanel
{
    public abstract class UIAbstractAnimProfileBar : UIBehaviour
    {
        protected class ProfileStack
        {
            protected const int NEW_MARK_NODATA = -1;
        }

        protected Sequence sequenceAnim;
        protected Queue<ProfileStack> Stack = new Queue<ProfileStack>();
        protected CompositeDisposable ActivateDisposables = new CompositeDisposable();
        
        public abstract void PlayProfileChangedAnimation();
        protected abstract void PopProfile();
        protected abstract void ResetAnimateInfluence();
        
        public void SetIgnoreNextProfileChange()
        {
            PopProfile();
        }

        public void SkipProfileChangedAnimation()
        {
            if (Stack.Count > 0) Stack.Dequeue();
            ResetAnimateInfluence();
        }
        
        protected override void OnDisable()
        {
            DeactivateAnim();
            ActivateDisposables.Clear();
        }

        protected override void OnDestroy()
        {
            DeactivateAnim();
        }
        
        protected void DeactivateAnim()
        {
            if (SequenceAnimIsValid())
            {
                sequenceAnim.Kill(false);
                sequenceAnim = null;
                Stack.Clear();
            }
        }
        
        protected bool SequenceAnimIsValid()
        {
            return sequenceAnim != null && sequenceAnim.IsActive();
        }
    }
}