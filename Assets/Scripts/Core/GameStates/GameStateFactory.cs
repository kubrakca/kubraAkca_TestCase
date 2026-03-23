using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GameStates
{
    /// <summary>
    /// Owns high-level game flow: registers state behaviours, switches active state, holds session data (selected level, last run outcome).
    /// </summary>
    public class GameStateFactory : MonoBehaviour
    {
        #region SerializeField

        [SerializeField] private List<GameState> states;

        #endregion

        #region Public Fields

        public int SelectedLevelIndex { get; set; }
        public bool IsLevelCompleted { get; set; }

        public float LastCompletionRemainingTime { get; set; }
        public float LastCompletionTimeLimit { get; set; }

        #endregion

        #region Private Fields

        private Dictionary<Type, GameState> _stateDict;
        private GameState _currentState;

        #endregion

        #region Unity Lifecycle

        /// <summary>Wires each <see cref="GameState"/> to this factory and disables them until entered.</summary>
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

        #endregion

        #region Public Methods

        /// <summary>Exits current state (if any) and enters the requested state type.</summary>
        public void ChangeState<T>() where T : GameState
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = _stateDict[typeof(T)];
            _currentState.Enter();
        }

        #endregion
    }
}
