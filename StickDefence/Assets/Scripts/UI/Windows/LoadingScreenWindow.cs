using System;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Windows
{
    public class LoadingScreenWindow : Window
    {
        [SerializeField] private UIButton _next;
        [SerializeField] private GameObject _pcTutorial;
        
        public event Action EndTransitAnim;

        protected override void OnActivate()
        {
            base.OnActivate();
            _next.OnClickAsObservable.Subscribe(_ => OnNext()).AddTo(ActivateDisposables);
        }

        public void ShowWebgl(bool psTutorial)
        {
            _pcTutorial.SetActive(psTutorial);
        }


        private void OnNext()
        {
            EndTransitAnim?.Invoke();
        }
    }
}