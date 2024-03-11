using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.IAP;
using Models.Player;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;

namespace Models.Controllers
{
    public class GameSceneControl : MonoBehaviour
    {
        [SerializeField] private GameObject _stageMap;
        [SerializeField] private GameObject _lobby;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly IPlayerRoot _player;
        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ISoundManager _soundManager;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        
        public void OnEnable()
        {
            _coreStateMachine.GameStateMachine.SubscriptionAction(GameStateEnum.Lobby, OnLobbyState);
            _coreStateMachine.GameStateMachine.SubscriptionAction(GameStateEnum.StageBattle, OnStageBattleState);
            _coreStateMachine.GameStateMachine.SetGameState(GameStateEnum.Lobby);
            _soundManager.PlayMusic();
        }

        private void OnDisable()
        {
            _coreStateMachine.GameStateMachine.UnSubscriptionAction(GameStateEnum.Lobby, OnLobbyState);
            _coreStateMachine.GameStateMachine.UnSubscriptionAction(GameStateEnum.StageBattle, OnStageBattleState);
            _disposables.Clear();
        }

        private void OnLobbyState()
        {
            _lobby.SetActive(true);
            _windowManager.Show<LobbyWindow>();
            _windowManager.Show<TopPanelWindow>(WindowPriority.TopPanel).SetTopLobbyState();
        }

        private void OnStageBattleState()
        {
            _stageMap.SetActive(true);
            _lobby.SetActive(false);
            _windowManager.FindWindow<TopPanelWindow>().SetTopGameState(); 
        }
    }
}