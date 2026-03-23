using Screens;
using UI;
using UnityEngine;
using Zenject;

namespace Core.GameStates
{
    public class StartGameState : GameState
    {
        [Inject] private ILevelService _levelService;
        private StartScreen _startScreen;

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

        private void HandleLevelSelected(int levelIndex)
        {
            Context.SelectedLevelIndex = levelIndex;
            Debug.Log($"Level {levelIndex} selected");
        }

        private void HandlePlayRequested()
        {
            Context.ChangeState<PlayingGameState>();
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
    }
}
