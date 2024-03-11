using System;
using DG.Tweening;
using Tools.GameTools;
using UI.UIManager;
using UnityEngine;

namespace UI.Windows
{
    public class FadeWindow : Window
    {
        [SerializeField] private DOTweenAnimation _doTweenAnimation;

        public override WindowPriority Priority => WindowPriority.LoadScene;

        private Action _fadeSceneDelegate;
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _fadeSceneDelegate = null;
        }

        public void OpenFade(Action endBack = null)
        {
            _fadeSceneDelegate = endBack;
            _doTweenAnimation.DORestartById(StringsHelper.Helper.Open);
            _doTweenAnimation.DOPlayById(StringsHelper.Helper.Open);
        }

        public void CloseFade(Action endBack = null)
        {
            _fadeSceneDelegate = endBack;
            _doTweenAnimation.DORestartById(StringsHelper.Helper.Close);
            _doTweenAnimation.DOPlayById(StringsHelper.Helper.Close);
        }
        
        public void EndOpenFade()
        {
            _fadeSceneDelegate?.Invoke();
            Debug.Log("Open scene Fade ended");
            _manager.Hide(this);
        }
        
        public void EndCloseFade()
        {
            _fadeSceneDelegate?.Invoke();
            Debug.Log("Close scene Fade ended");
        }
    }
}