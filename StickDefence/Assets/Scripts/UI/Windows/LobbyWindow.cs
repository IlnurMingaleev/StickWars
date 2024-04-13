using Enums;
using Models.DataModels;
using Models.IAP;
using Models.Player;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.Common;
using UI.Content.Lobby;
using UI.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace UI.Windows
{
    public class LobbyWindow : Window
    {
        [SerializeField] private UIButton _battleSelectionMapButton;
        [SerializeField] private CoinSmeltingPanel _coinSmeltingPanel;
        [SerializeField] private UpgradeBasePanel _upgradeBasePanel;
        [SerializeField] private StoragePanel _storagePanel;

        [Inject] private IDataCentralService _dataCentralService; 
        [Inject] private ICoreStateMachine _coreStateMachine;
        [Inject] private IPlayer _player;
        [Inject] private IIAPService _iapService;
        private ReactiveProperty<bool> _clickableLobbyBuilds = new(false);
        
        public IReadOnlyReactiveProperty<bool> CanClickableLobby => _clickableLobbyBuilds;
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _manager.ClearLastWindows();
            _manager.AddCurrentWindow(this);
            _manager.GetWindow<TopPanelWindow>().SetTopLobbyState();

            _battleSelectionMapButton.OnClickAsObservable.Subscribe(_ =>SetupStage())
                .AddTo(ActivateDisposables);
            
            _clickableLobbyBuilds.Value = true;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _clickableLobbyBuilds.Value = false;
        }

        private void OnBattleSelectionMap()
        {
            _manager.Show<BattleSelectionMapWindow>();
            _manager.Hide(this);
        }

        public void OpenCoinSmelting()
        {
            _clickableLobbyBuilds.Value = false;
            _coinSmeltingPanel.Open(() => _clickableLobbyBuilds.Value = true);
        }
        
        public void OpenUpgradeBase()
        {
            _clickableLobbyBuilds.Value = false;
            _upgradeBasePanel.Open(() => _clickableLobbyBuilds.Value = true);
        }
        
        public void OpenStorage()
        {
            _clickableLobbyBuilds.Value = false;
            _storagePanel.Open(() => _clickableLobbyBuilds.Value = true);
        }

        public void SetupStage()
        {
            var isShowing = _iapService.ShowCommercialBreak();

            if (!isShowing)
            {
                _manager.Show<FadeWindow>().CloseFade(EndStartBattleFade);
            }
        }
        private void EndStartBattleFade()
        {
            _coreStateMachine.GameStateMachine.SetGameState(GameStateEnum.StageBattle);
            _coreStateMachine.BattleStateMachine.SetBattleState(BattleStateEnum.LoadBattle);
            _manager.Hide(this);
        }
    }
}