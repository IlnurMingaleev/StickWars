using System;
using TMPro;
using UI.Common;
using UI.UIManager;
using UniRx;
using UnityEngine;

namespace UI.Windows
{
    public class ChoiceMessageWindow : Window
    {
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private UIButton _okButton;
        [SerializeField] private UIButton _closeButton;

        public override WindowPriority Priority { get; set; } = WindowPriority.AboveTopPanel;
        
        public void Init(string text, Action success)
        {
            _messageLabel.text = text;
            _okButton.OnClickAsObservable.Subscribe(_ =>
            {
                success?.Invoke();
                _manager.Hide(this);
            }).AddTo(ActivateDisposables);
            
            _closeButton.OnClickAsObservable.Subscribe(_ =>
            {
                _manager.Hide(this);
            }).AddTo(ActivateDisposables);
        }
    }
}