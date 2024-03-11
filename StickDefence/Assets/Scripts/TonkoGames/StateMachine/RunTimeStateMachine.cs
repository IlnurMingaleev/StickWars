using System;
using System.Collections.Generic;
using TonkoGames.StateMachine.Enums;
using Tools.GameTools;
using UniRx;

namespace TonkoGames.StateMachine
{
    public interface IRunTimeStateMachine
    {
        RunTimeStateEnum LastRunTimeState { get; }
        IReadOnlyReactiveProperty<RunTimeStateEnum> RunTimeState { get; }
        void SetRunTimeState(RunTimeStateEnum runTimeStateEnum);
        void SubscriptionAction(RunTimeStateEnum runTimeStateEnum, Action action);
        void UnSubscriptionAction(RunTimeStateEnum runTimeStateEnum, Action action);
    }
    
    public class RunTimeStateMachine : IRunTimeStateMachine
    {
        private readonly ReactiveProperty<RunTimeStateEnum> _runTimeState = new(RunTimeStateEnum.Pause);
        private readonly Dictionary<RunTimeStateEnum, List<Action>> _actionSubscribes = new();
        public IReadOnlyReactiveProperty<RunTimeStateEnum> RunTimeState => _runTimeState;
        public RunTimeStateEnum LastRunTimeState { get; private set; }


        public RunTimeStateMachine()
        {
            foreach (RunTimeStateEnum runTime in Enum.GetValues(typeof(RunTimeStateEnum)))
            {
                _actionSubscribes.Add(runTime, new List<Action>());
            }
            _runTimeState.Subscribe(ChangedState);
        }
        
        public void SetRunTimeState(RunTimeStateEnum runTimeStateEnum)
        {
            Debugger.Log($"SetPlayTime: {runTimeStateEnum.ToString()}");
            LastRunTimeState = _runTimeState.Value;
            _runTimeState.Value = runTimeStateEnum;
        }

        private void ChangedState(RunTimeStateEnum runTimeStateEnum)
        {
            foreach (var action in _actionSubscribes[runTimeStateEnum])
            {
                action?.Invoke();
            }
        }

        public void SubscriptionAction(RunTimeStateEnum runTimeStateEnum, Action action) => _actionSubscribes[runTimeStateEnum].Add(action);
        
        public void UnSubscriptionAction(RunTimeStateEnum runTimeStateEnum, Action action) => _actionSubscribes[runTimeStateEnum].Remove(action);
    }
}