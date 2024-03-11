using TMPro;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Windows
{
    public class PopupMessageWindow : Window
    {
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private UIButton _okButton;
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _okButton.OnClickAsObservable.Subscribe(_ => OnOk()).AddTo(ActivateDisposables);
        }

        public void Init(string titleText, string messageText)
        {
            _titleLabel.text = titleText;
            _messageLabel.text = messageText;
        }

        private void OnOk()
        {
            _manager.Hide(this);
        }
    }
}