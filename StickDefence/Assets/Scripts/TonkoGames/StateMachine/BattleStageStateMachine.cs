using System;
using System.Collections.Generic;
using System.Linq;
using TonkoGames.StateMachine.Enums;
using UniRx;

namespace TonkoGames.StateMachine
{
    public interface IBattleStateMachineCheat
    {
        void CheatEndBattle(bool value,bool isExitGame=false);
    }
    
    public interface IBattleStateMachine : IBattleStateMachineCheat
    {
        event Action<bool,bool> EndBattle;
        BattleStateEnum LastBattleState{ get; }
        IReadOnlyReactiveProperty<BattleStateEnum> BattleState { get; }
        void SetBattleState(BattleStateEnum gameStateEnum);
        void SubscriptionAction(BattleStateEnum state, Action action);
        void UnSubscriptionAction(BattleStateEnum state, Action action);

        void OnEndBattle(bool value, bool isExitGame=false);
    }

    public class BattleStateMachine : IBattleStateMachine
    {
        private readonly ReactiveProperty<BattleStateEnum> _battleState = new ReactiveProperty<BattleStateEnum>(BattleStateEnum.None);
        private readonly Dictionary<BattleStateEnum, List<Action>> _actionSubscribes = new();

        public IReadOnlyReactiveProperty<BattleStateEnum> BattleState => _battleState;
        public BattleStateEnum LastBattleState{ get; private set; }

        public event Action<bool,bool> EndBattle; 

        public BattleStateMachine()
        {
            foreach (BattleStateEnum state in Enum.GetValues(typeof(BattleStateEnum)))
            {
                _actionSubscribes.Add(state, new List<Action>());
            }
            _battleState.Subscribe(ChangedState);
        }

        public void SetBattleState(BattleStateEnum state)
        {
            LastBattleState = _battleState.Value;
            _battleState.Value = state;
        }
        
        private void ChangedState(BattleStateEnum state)
        {
            foreach (var action in _actionSubscribes[state].ToList())
            {
                action?.Invoke();
            }
        }

        public void SubscriptionAction(BattleStateEnum state, Action action) => _actionSubscribes[state].Add(action);
        
        public void UnSubscriptionAction(BattleStateEnum state, Action action) => _actionSubscribes[state].Remove(action);

        public void OnEndBattle(bool value,bool isExitGame = false) => EndBattle?.Invoke(value, isExitGame);
        
        public void CheatEndBattle(bool value,bool isExitGame = false) => EndBattle?.Invoke(value,isExitGame);
    }
}