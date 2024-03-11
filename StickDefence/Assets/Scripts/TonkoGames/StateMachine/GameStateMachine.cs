using System;
using System.Collections.Generic;
using TonkoGames.StateMachine.Enums;
using UniRx;

namespace TonkoGames.StateMachine
{
    public interface IGameStateMachine
    {
        GameStateEnum LastGameState { get; }
        IReadOnlyReactiveProperty<GameStateEnum> GameState { get; }
        void SetGameState(GameStateEnum gameStateEnum);
        void SubscriptionAction(GameStateEnum gameState, Action action);
        void UnSubscriptionAction(GameStateEnum gameState, Action action);
    }
    public class GameStateMachine : IGameStateMachine
    {
        private readonly ReactiveProperty<GameStateEnum> _gameState = new ReactiveProperty<GameStateEnum>();
        private readonly Dictionary<GameStateEnum, List<Action>> _actionSubscribes = new();

        public IReadOnlyReactiveProperty<GameStateEnum> GameState => _gameState;

        private GameStateEnum _lastGameState = GameStateEnum.Lobby;
        public GameStateEnum LastGameState => _lastGameState;


        public GameStateMachine()
        {
            foreach (GameStateEnum gameState in Enum.GetValues(typeof(GameStateEnum)))
            {
                _actionSubscribes.Add(gameState, new List<Action>());
            }
            _gameState.Subscribe(ChangedState);
        }

        public void SetGameState(GameStateEnum gameStateEnum)
        {
            _lastGameState = _gameState.Value;

            _gameState.Value = gameStateEnum;
        }
        
        private void ChangedState(GameStateEnum gameState)
        {
            foreach (var action in _actionSubscribes[gameState])
            {
                action?.Invoke();
            }
        }

        public void SubscriptionAction(GameStateEnum gameState, Action action) => _actionSubscribes[gameState].Add(action);

        public void UnSubscriptionAction(GameStateEnum gameState, Action action) => _actionSubscribes[gameState].Remove(action);
    }
}