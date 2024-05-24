using Models.Controllers;
using TonkoGames.Controllers.Core;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using Models.DataModels;
using Models.Player;
using TonkoGames.Sound;
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
        private readonly SceneInstances _sceneInstances;
        private readonly ISoundManager _soundManager;
        
        public BattleResult(PlayerFortressInstantiate playerFortressInstantiate, IWindowManager windowManager,
            ConfigManager configManager, IPlayer player, IDataCentralService dataCentralService,
            ICoreStateMachine coreStateMachine, SceneInstances sceneInstances, ISoundManager soundManager)
        {
            _playerFortressInstantiate = playerFortressInstantiate;
            _windowManager = windowManager;
            _configManager = configManager;
            _player = player;
            _dataCentralService = dataCentralService;
            _coreStateMachine = coreStateMachine;
            _sceneInstances = sceneInstances;
            _soundManager = soundManager;
        }

        public void OnLoadBattleState()
        {
            _isResurrected = false;
        }
            
        public void OnLoseEvent(bool isExitGame = false)
        {
            ShowPlayResult(0, false, !_isResurrected, isExitGame);
            if(!isExitGame) _isResurrected = true;

        }
        
        public void OnWinEvent()
        {
            var percentWin = _playerFortressInstantiate.GetDeltaHealth();
            UpgradeStageIndex();
            int stars = 0;
            if (percentWin > 0.7)
                stars = 3;
            else if (percentWin > 0.3)
                stars = 2;
            else
                stars = 1;

            ShowPlayResult(stars, true);
        }
        
        private void ShowPlayResult(int stars, bool isWin, bool isResurrect = false, bool isExitGame = false)
        {
            var playResultWindow = _windowManager.Show<PlayResultWindow>(WindowPriority.AboveTopPanel);
            var rewardModel = _configManager.MapStageSO.MapStageRewardModels[ _dataCentralService.PumpingDataModel.StageLoadType.Value];

            rewardModel.Coin -= (rewardModel.Coin / 3) * (3 - stars);

            var rewardContains = new RewardContains()
            {
                Coin = rewardModel.Coin,
                Gem = rewardModel.Gem
            };
            /*var mapStageData = _dataCentralService.MapStageDataModel.GetMapStageData(_dataCentralService.PumpingDataModel.StageLoadType.Value);

            if (!mapStageData.IsCompleted)
            {
                mapStageData.IsCompleted = true;
                _dataCentralService.MapStageDataModel.UpdateMapStageData(mapStageData);
                _dataCentralService.SaveFull();
                
            }*/
            if (isWin)
            {
                
                playResultWindow.SetWin(rewardContains, stars, OnPlayResultWindowClaim,Continue,_sceneInstances);
                _soundManager.PlayWinSourceOneShot();
            }
            else
            {
                playResultWindow.SetLose(rewardContains, isResurrect,OnPlayResultWindowClaim, Resurrect, isExitGame);
                _soundManager.PlayLooseSourceOneShot();
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
        private void UpgradeStageIndex()
        {
            _dataCentralService.PumpingDataModel.SetStageIndex((int) _dataCentralService.PumpingDataModel.StageLoadType.Value + 1);
            _dataCentralService.SaveFull();
        }
    }
}