using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GameStates
{
    public class GameStateFactory : MonoBehaviour
    {
        [SerializeField] private List<GameState> states;

        private Dictionary<Type, GameState> _stateDict;
        private GameState _currentState;

        public int SelectedLevelIndex { get; set; }
        public bool IsLevelCompleted { get; set; }

        private void Awake()
        {
            _stateDict = new Dictionary<Type, GameState>();

            foreach (var state in states)
            {
                state.SetContext(this);
                _stateDict[state.GetType()] = state;
                state.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            ChangeState<StartGameState>();
        }

        public void ChangeState<T>() where T : GameState
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = _stateDict[typeof(T)];
            _currentState.Enter();
        }
    }
}
