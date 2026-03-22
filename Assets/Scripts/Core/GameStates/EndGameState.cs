using UnityEngine;
using Zenject;
using UI;

namespace Core.GameStates
{
    public class EndGameState : GameState
    {
        [Inject] private ILevelService _levelService;

        private EndScreen _endScreen;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter()
        {
            base.Enter();

            if (Context.IsLevelCompleted)
                _levelService.SaveProgress();

            _endScreen = UIService.Show<EndScreen>();
            _endScreen.Initialize(Context.IsLevelCompleted);
            _endScreen.OnContinueClicked += HandleContinue;
            _endScreen.OnReplayClicked += HandleReplay;

            Debug.Log(Context.IsLevelCompleted ? "Level Completed!" : "Level Failed!");
        }

        private void HandleContinue()
        {
            Context.ChangeState<StartGameState>();
        }

        private void HandleReplay()
        {
            Context.ChangeState<PlayingGameState>();
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
    }
}
