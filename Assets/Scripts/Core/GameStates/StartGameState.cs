using Screens;
using UI;
using UnityEngine;
using Zenject;

namespace Core.GameStates
{
    /// <summary>Main menu: level picker and play button; updates <see cref="GameStateFactory.SelectedLevelIndex"/>.</summary>
    public class StartGameState : GameState
    {
        #region Dependency Injection

        [Inject] private ILevelService _levelService;

        #endregion

        #region Private Fields

        private StartScreen _startScreen;

        #endregion

        #region Public Methods

        public override void Enter()
        {
            base.Enter();

            Context.SelectedLevelIndex = _levelService.CurrentLevelIndex;

            _startScreen = UIService.Show<StartScreen>();

            if (_startScreen != null)
            {
                _startScreen.OnLevelSelected += HandleLevelSelected;
                _startScreen.OnPlayRequested += HandlePlayRequested;
            }
        }

        public override void Exit()
        {
            if (_startScreen != null)
            {
                _startScreen.OnLevelSelected -= HandleLevelSelected;
                _startScreen.OnPlayRequested -= HandlePlayRequested;
            }

            UIService.Hide<StartScreen>();
            base.Exit();
        }

        #endregion

        #region Private Methods

        private void HandleLevelSelected(int levelIndex)
        {
            Context.SelectedLevelIndex = levelIndex;
            Debug.Log($"Level {levelIndex} selected");
        }

        private void HandlePlayRequested()
        {
            Context.ChangeState<PlayingGameState>();
        }

        #endregion
    }
}
