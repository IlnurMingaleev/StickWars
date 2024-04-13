using Enums;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.Player;
using UI.Content.Rewards;
using UI.UIManager;
using UI.Windows;

namespace Models.Battle
{
    public class BattleResult
    {
        private bool _isResurrected = false;

        private readonly PlayerFortressInstantiate _playerFortressInstantiate;
        private readonly IWindowManager _windowManager;
        private readonly ConfigManager _configManager;
        private readonly IPlayer _player;
        private readonly IDataCentralService _dataCentralService;
        private readonly ICoreStateMachine _coreStateMachine;
        
        public BattleResult(PlayerFortressInstantiate playerFortressInstantiate, IWindowManager windowManager, 
            ConfigManager configManager, IPlayer player, IDataCentralService dataCentralService, ICoreStateMachine coreStateMachine)
        {
            _playerFortressInstantiate = playerFortressInstantiate;
            _windowManager = windowManager;
            _configManager = configManager;
            _player = player;
            _dataCentralService = dataCentralService;
            _coreStateMachine = coreStateMachine;
        }

        public void OnLoadBattleState()
        {
            _isResurrected = false;
        }
            
        public void OnLoseEvent()
        {
            ShowPlayResult(0, false, !_isResurrected);
            _isResurrected = true;
        }
        
        public void OnWinEvent()
        {
            var percentWin = _playerFortressInstantiate.GetDeltaHealth();

            int stars = 0;
            if (percentWin > 0.7)
                stars = 3;
            else if (percentWin > 0.3)
                stars = 2;
            else
                stars = 1;

            ShowPlayResult(stars, true);
        }
        
        private void ShowPlayResult(int stars, bool isWin, bool isResurrect = false)
        {
            var playResultWindow = _windowManager.Show<PlayResultWindow>(WindowPriority.AboveTopPanel);
            var rewardModel = _configManager.MapStageSO.MapStageRewardModels[ _dataCentralService.PumpingDataModel.StageLoadType.Value];

            rewardModel.Coin -= (rewardModel.Coin / 3) * (3 - stars);

            var rewardContains = new RewardContains()
            {
                Coin = rewardModel.Coin,
                Gem = rewardModel.Gem
            };
            var mapStageData = _dataCentralService.MapStageDataModel.GetMapStageData(_dataCentralService.PumpingDataModel.StageLoadType.Value);

            if (!mapStageData.IsCompleted)
            {
                mapStageData.IsCompleted = true;
                _dataCentralService.MapStageDataModel.UpdateMapStageData(mapStageData);
                _dataCentralService.SaveFull();
                
            }
            if (isWin)
            {
                
                playResultWindow.SetWin(rewardContains, stars, OnPlayResultWindowClaim,Continue);
            }
            else
            {
                playResultWindow.SetLose(rewardContains, isResurrect,OnPlayResultWindowClaim, Resurrect);
            }
        }

        private void OnPlayResultWindowClaim()
        {
           
            var playResultWindow = _windowManager.GetWindow<PlayResultWindow>();
            var fadeWindow = _windowManager.Show<FadeWindow>(WindowPriority.LoadScene);
        
            fadeWindow.CloseFade(() =>
            {
                _coreStateMachine.GameStateMachine.SetGameState(GameStateEnum.Lobby);
                _windowManager.Hide(playResultWindow);
                fadeWindow.OpenFade();
                _coreStateMachine.BattleStateMachine.SetBattleState(BattleStateEnum.None);
            });
        }
        
        private void Resurrect()
        {
            _playerFortressInstantiate.Resurrect();
            _windowManager.Show<BottomPanelWindow>();
            _coreStateMachine.RunTimeStateMachine.SetRunTimeState(RunTimeStateEnum.Play);
        }

        private void Continue()
        {
            _windowManager.Hide<PlayResultWindow>();
            _windowManager.GetWindow<LobbyWindow>().SetupStage();
        }
    }
}