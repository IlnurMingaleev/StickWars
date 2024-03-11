using System;
using TonkoGames.StateMachine.Enums;
using UniRx;

namespace TonkoGames.StateMachine
{
    public interface ISceneStateMachine
    {
        void SetScenesState(ScenesStateEnum scenesStateEnum);
        void OnSceneEndLoadFade();
        void OnSceneEndLoad();
        IReadOnlyReactiveProperty<ScenesStateEnum> ScenesState { get; }
        event Action<ScenesStateEnum> SceneEndLoadFade;
        
        event Action<ScenesStateEnum> SceneEndLoad;
    }
    public class SceneStateMachine : ISceneStateMachine
    {
        private ReactiveProperty<ScenesStateEnum> _scenesState = new ReactiveProperty<ScenesStateEnum>(ScenesStateEnum.Base);
        
        public IReadOnlyReactiveProperty<ScenesStateEnum> ScenesState => _scenesState;
        
        public event Action<ScenesStateEnum> SceneEndLoadFade;
        public event Action<ScenesStateEnum> SceneEndLoad;
        
        
        public void SetScenesState(ScenesStateEnum scenesStateEnum)
        {
            _scenesState.Value = scenesStateEnum;
        }
        
        public void OnSceneEndLoadFade()
        {
            SceneEndLoadFade?.Invoke(_scenesState.Value);
        }
        
        public void OnSceneEndLoad()
        {
            SceneEndLoad?.Invoke(_scenesState.Value);
        }
    }
}