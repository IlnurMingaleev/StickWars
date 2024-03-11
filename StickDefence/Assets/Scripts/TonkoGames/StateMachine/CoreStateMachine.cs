using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace TonkoGames.StateMachine
{

    public interface ICoreStateMachineCheat
    {
        IBattleStateMachineCheat BattleStateMachineCheat { get; }
    }
    
    public interface ICoreStateMachine : ICoreStateMachineCheat
    {
        #region SubStates

        IGameStateMachine GameStateMachine { get; }
        ITutorialStateMachine TutorialStateMachine { get; }
        IBattleStateMachine BattleStateMachine { get; }
        IRunTimeStateMachine RunTimeStateMachine { get; }
        ISceneStateMachine SceneStateMachine { get; }

        #endregion
    }

    public class CoreStateMachine : ICoreStateMachine
    {
        public event Action ResetProgress;

        private RunTimeStateMachine  _runTimeStateMachine  = new();
        private GameStateMachine     _gameStateMachine     = new();
        private TutorialStateMachine _tutorialStateMachine = new();
        private BattleStateMachine   _battleStateMachine   = new();
        private SceneStateMachine    _sceneStateMachine    = new();

        public IGameStateMachine GameStateMachine => _gameStateMachine;

        public ITutorialStateMachine TutorialStateMachine => _tutorialStateMachine;

        public IBattleStateMachine BattleStateMachine => _battleStateMachine;

        public IBattleStateMachineCheat BattleStateMachineCheat => _battleStateMachine;

        public IRunTimeStateMachine RunTimeStateMachine => _runTimeStateMachine;

        public ISceneStateMachine SceneStateMachine => _sceneStateMachine;
        
        public void OnResetProgress() => ResetProgress?.Invoke();
    }
}