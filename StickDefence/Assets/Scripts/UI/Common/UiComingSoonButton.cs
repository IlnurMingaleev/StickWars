using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Common
{
    public class UiComingSoonButton : UIBehaviour
    {
        [SerializeField] private UIButton _selfButton;
        [SerializeField] private GameObject _soonGO;

        protected override void OnEnable()
        {
            base.OnEnable();
            _selfButton.OnClick
                .AsObservable()
                .TakeUntilDisable(this)
                .Subscribe(uib =>
                {
                    OnSelfButton();
                });
        }


        private void OnSelfButton()
        {
            _soonGO.gameObject.SetActive(true);
        }
    }
}