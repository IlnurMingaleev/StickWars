using TonkoGames.MultiScene;
using TonkoGames.StateMachine;
using TonkoGames.StateMachine.Enums;
using UI.UIManager;
using UI.Windows;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Models.Controllers
{
    public class ScenesControllerModel : IInitializable
    {
        private ScenesStateEnum _scene = ScenesStateEnum.Base;
        [Inject] private readonly IMultiSceneManager _multiSceneManager;
        [Inject] private readonly ICoreStateMachine _coreStateMachine;
        [Inject] private readonly IWindowManager _windowManager;
        private FadeWindow _fadeWindow;
        
        public void Initialize()
        {
            _coreStateMachine.SceneStateMachine.ScenesState.SkipLatestValueOnSubscribe().Subscribe(CurrentSceneSwitches);
        }
        private void CurrentSceneSwitches(ScenesStateEnum scene)
        {        
            _scene = scene;
            if (_fadeWindow == null)
            {
                _fadeWindow = _windowManager.GetWindow<FadeWindow>();
            }
            
            if (_fadeWindow.IsShowing)
            {
                _fadeWindow.CloseFade(EndCloseFade);
            }
            else
            {
                _windowManager.Show(_fadeWindow);
                _fadeWindow.CloseFade(EndCloseFade);
            }
        }

        private void EndCloseFade()
        {
            _coreStateMachine.SceneStateMachine.OnSceneEndLoad();
            _multiSceneManager.LoadScene(_scene, NextSceneEndLoad);
        }
        
        private void EndOpenFade() => _coreStateMachine.SceneStateMachine.OnSceneEndLoadFade();
        
        private void NextSceneEndLoad()
        {
            _multiSceneManager.UnloadLastScene();
            _multiSceneManager.SetActiveLastLoadScene();
            _fadeWindow.OpenFade(EndOpenFade);
        }
    }
}