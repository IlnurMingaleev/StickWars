using Cysharp.Threading.Tasks;
using Models.Controllers;
using Models.DataModels;
using Models.IAP;
using Models.Player;
using TonkoGames.Controllers.Tutorial;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.UIManager;
using UI.Windows;
using UnityEngine;
using VContainer;

namespace TonkoGames.Controllers.Core
{
    public class GameInstance : MonoBehaviour
    {
        [Inject] private readonly IAPService _iapService;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        [Inject] private readonly DataCentralService _dataCentralService;
        [Inject] private readonly IPlayerRoot _player;
        [Inject] private readonly ILobbyModelsRoot _lobbyModels;
        [Inject] private readonly ConfigManager _configManager;
        
        private readonly TutorialService _tutorialService = new();
        
        private bool _dataLoad = false;

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GamePush.GP_Game.OnPause += OnPause;
            GamePush.GP_Game.OnResume += OnResume;
#endif
            DontDestroyOnLoad(this);
            
            SetupFrameTimes();
            _coreStateMachine.SceneStateMachine.SceneEndLoadFade += SceneEndLoadFade;
            _coreStateMachine.SceneStateMachine.SceneEndLoad += SceneEndLoad;
            
            RegServices().Forget();

            _windowManager.GetWindow<TutorialWindow>();
        }

        
        private void SetupFrameTimes()
        {
            Application.targetFrameRate = 60;
        }

        private async UniTaskVoid RegServices()
        {
            await UniTask.DelayFrame(10);
            await _dataCentralService.LoadData();
            await UniTask.DelayFrame(10);
            _dataLoad = true;
            _player.CheckDailyModel();
            _player.Init();
            _lobbyModels.Init();
            _dataCentralService.SaveFull();
            _coreStateMachine.TutorialStateMachine.InitTutorials(_dataCentralService.SubData);

            _coreStateMachine.SceneStateMachine.SetScenesState(ScenesStateEnum.Game);
            
            _iapService.PaymentModel.Fetch();
        }

        private void LogoScreenShown()
        { 
            _coreStateMachine.SceneStateMachine.SetScenesState(ScenesStateEnum.Game);
        }

        private void SceneEndLoadFade(ScenesStateEnum scenesStateEnum)
        {
            if (scenesStateEnum.Equals(ScenesStateEnum.Game))
            {
                _coreStateMachine.SceneStateMachine.SceneEndLoadFade -= SceneEndLoadFade;
                
                _iapService.SetGameLoadingFinished();
            }
        }
        
        private void SceneEndLoad(ScenesStateEnum scenesStateEnum)
        {
            _coreStateMachine.SceneStateMachine.SceneEndLoad -= SceneEndLoad;
            /*var loadingScreenWindow = _windowManager.GetWindow<LoadingScreenWindow>();
            if (loadingScreenWindow.IsShowing)
            {
                _windowManager.Hide<LoadingScreenWindow>().EndTransitAnim -= LogoScreenShown;
            }*/
  
            _tutorialService.Init(_coreStateMachine, _dataCentralService, _windowManager, _configManager);
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!_dataLoad)
            {
                return;
            }
            
            if (isPaused)
            {
                _dataCentralService.SaveStatsDataModel();
            }
            else
            {
                _player.CheckDailyModel();
            }
        }
 
        private void OnPause() => AudioListener.pause = true;
        private void OnResume() => AudioListener.pause = false;
    }
}