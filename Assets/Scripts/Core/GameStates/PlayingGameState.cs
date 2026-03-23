using UI;
using UnityEngine;
using Zenject;

namespace Core.GameStates
{
    /// <summary>Active round: HUD timer, spawned level, input; handles pause, win (all stars matched), or time fail.</summary>
    public class PlayingGameState : GameState
    {
        #region Dependency Injection

        [Inject] private ILevelService _levelService;
        [Inject] private LevelSpawner _levelSpawner;
        [Inject] private GameplayController _gameplayController;

        #endregion

        #region Private Fields

        private GameScreen _gameScreen;
        private SettingsScreen _settingsScreen;

        #endregion

        #region Public Methods

        public override void Enter()
        {
            base.Enter();

            var levelData = _levelService.GetLevelData(Context.SelectedLevelIndex);

            if (levelData == null)
            {
                Debug.LogError($"LevelData not found for index {Context.SelectedLevelIndex}");
                Context.ChangeState<StartGameState>();
                return;
            }

            _gameScreen = UIService.Show<GameScreen>();
            _gameScreen.Initialize(levelData);
            _gameScreen.OnTimerExpired += HandleTimerExpired;
            _gameScreen.OnPauseClicked += HandlePause;

            _levelSpawner.SpawnLevel(levelData);

            _gameplayController.Initialize(levelData);
            _gameplayController.OnAllStarsMatched += HandleLevelCompleted;

            Debug.Log($"Playing Level {levelData.levelNumber}");
        }

        public override void Exit()
        {
            if (_gameScreen != null)
            {
                _gameScreen.OnTimerExpired -= HandleTimerExpired;
                _gameScreen.OnPauseClicked -= HandlePause;
                _gameScreen.StopTimer();
            }

            if (_settingsScreen != null)
            {
                _settingsScreen.OnResumeClicked -= HandleResume;
                _settingsScreen.OnHomeClicked -= HandleBackToHome;
                UIService.Hide<SettingsScreen>();
                _settingsScreen = null;
            }

            _gameplayController.OnAllStarsMatched -= HandleLevelCompleted;
            _gameplayController.Deactivate();

            _levelSpawner.ClearLevel();
            UIService.Hide<GameScreen>();
            base.Exit();
        }

        #endregion

        #region Private Methods

        private void HandlePause()
        {
            _gameScreen.PauseTimer();
            _gameplayController.Deactivate();

            _settingsScreen = UIService.Show<SettingsScreen>();
            _settingsScreen.OnResumeClicked += HandleResume;
            _settingsScreen.OnHomeClicked += HandleBackToHome;
        }

        private void HandleResume()
        {
            if (_settingsScreen != null)
            {
                _settingsScreen.OnResumeClicked -= HandleResume;
                _settingsScreen.OnHomeClicked -= HandleBackToHome;
                UIService.Hide<SettingsScreen>();
                _settingsScreen = null;
            }

            _gameScreen.ResumeTimer();
            _gameplayController.Reactivate();
        }

        private void HandleBackToHome()
        {
            Context.ChangeState<StartGameState>();
        }

        private void HandleLevelCompleted()
        {
            Context.LastCompletionRemainingTime = _gameScreen.GetRemainingTime();
            var levelData = _levelService.GetLevelData(Context.SelectedLevelIndex);
            Context.LastCompletionTimeLimit = levelData != null ? levelData.timeLimit : 0f;

            _levelService.CompleteLevel(Context.SelectedLevelIndex);
            Context.IsLevelCompleted = true;
            Debug.Log("Level Completed!");
            Context.ChangeState<EndGameState>();
        }

        private void HandleTimerExpired()
        {
            Context.IsLevelCompleted = false;
            Debug.Log("Level Failed - Time's up!");
            Context.ChangeState<EndGameState>();
        }

        #endregion
    }
}
