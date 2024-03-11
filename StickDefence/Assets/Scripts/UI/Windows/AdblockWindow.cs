using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.UIManager;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Windows
{
    public class AdblockWindow : Window
    {
        [SerializeField] private Button _button;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;

        protected override void OnActivate()
        {
            base.OnActivate();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Pause);
            _button.onClick.AddListener(CloseWindow);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(_coreStateMachine.RunTimeStateMachine.LastRunTimeState);
            _button.onClick.RemoveAllListeners();
        }

        private void CloseWindow()
        {
            _manager.Hide(this);
        }
    }
}