using Models.Battle;
using TonkoGames.Sound;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.Fabrics;
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
        [SerializeField] private GameObject _lobby;
        [SerializeField] private GameObject _battleParent;
        [SerializeField] private GameObject _stageMapPrefab;
        
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly IPlayerRoot _player;
        [Inject] private readonly IIAPService _iapService;
        [Inject] private readonly IDataCentralService _dataCentralService;
        [Inject] private readonly ISoundManager _soundManager;
        [Inject] private readonly PrefabInject _prefabInject;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameObject _stageMap;

        public SceneInstances SceneInstances;
        
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
            if (_stageMap != null)
                Destroy(_stageMap);
            _battleParent.SetActive(false);
            
            /*var battleStageControl = _stageMap.GetComponent<BattleStageControl>();
            SceneInstances = _stageMap.GetComponent<SceneInstances>();
            battleStageControl.Init(this);*/
            _lobby.SetActive(true);
            _windowManager.Show<LobbyWindow>();
            _windowManager.Show<TopPanelWindow>(WindowPriority.TopPanel).SetTopLobbyState();
        }

        private void OnStageBattleState()
        {
            _prefabInject.InjectGameObject(_stageMap = Instantiate(_stageMapPrefab, _battleParent.transform));
            _battleParent.SetActive(true);
            
            _lobby.SetActive(false);
            _windowManager.FindWindow<TopPanelWindow>().SetTopGameState(); 
        }
    }
}