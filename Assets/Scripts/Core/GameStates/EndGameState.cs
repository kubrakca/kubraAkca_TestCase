using UnityEngine;
using Zenject;
using UI;

namespace Core.GameStates
{
    /// <summary>Post-level UI: win/lose panels, star score on success, persist progress when completed.</summary>
    public class EndGameState : GameState
    {
        #region Dependency Injection

        [Inject] private ILevelService _levelService;

        #endregion

        #region Private Fields

        private EndScreen _endScreen;

        #endregion

        #region Public Methods

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter()
        {
            base.Enter();

            if (Context.IsLevelCompleted)
                _levelService.SaveProgress();

            _endScreen = UIService.Show<EndScreen>();
            int stars = 0;
            if (Context.IsLevelCompleted)
                stars = EndScreen.ComputeStarRating(Context.LastCompletionRemainingTime, Context.LastCompletionTimeLimit);
            _endScreen.Initialize(Context.IsLevelCompleted, stars);
            _endScreen.OnContinueClicked += HandleContinue;
            _endScreen.OnReplayClicked += HandleReplay;

            Debug.Log(Context.IsLevelCompleted ? "Level Completed!" : "Level Failed!");
        }

        public override void Exit()
        {
            if (_endScreen != null)
            {
                _endScreen.OnContinueClicked -= HandleContinue;
                _endScreen.OnReplayClicked -= HandleReplay;
                UIService.Hide<EndScreen>();
                _endScreen = null;
            }

            base.Exit();
        }

        #endregion

        #region Private Methods

        private void HandleContinue()
        {
            Context.ChangeState<StartGameState>();
        }

        private void HandleReplay()
        {
            Context.ChangeState<PlayingGameState>();
        }

        #endregion
    }
}
