using Core;
using UI;
using UnityEngine;
using Zenject;

namespace Core.GameStates
{
    public class PlayingGameState : GameState
    {
        [Inject] private ILevelService _levelService;
        [Inject] private LevelSpawner _levelSpawner;
        [Inject] private GameplayController _gameplayController;

        private GameScreen _gameScreen;

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

            _levelSpawner.SpawnLevel(levelData);

            _gameplayController.Initialize(levelData);
            _gameplayController.OnAllStarsMatched += HandleLevelCompleted;

            Debug.Log($"Playing Level {levelData.levelNumber}");
        }

        private void HandleLevelCompleted()
        {
            _levelService.CompleteLevel(Context.SelectedLevelIndex);
            Debug.Log("Level Completed!");
            Context.ChangeState<EndGameState>();
        }

        private void HandleTimerExpired()
        {
            Debug.Log("Level Failed - Time's up!");
            Context.ChangeState<EndGameState>();
        }

        public override void Exit()
        {
            if (_gameScreen != null)
            {
                _gameScreen.OnTimerExpired -= HandleTimerExpired;
                _gameScreen.StopTimer();
            }

            _gameplayController.OnAllStarsMatched -= HandleLevelCompleted;
            _gameplayController.Deactivate();

            _levelSpawner.ClearLevel();
            UIService.Hide<GameScreen>();
            base.Exit();
        }
    }
}
