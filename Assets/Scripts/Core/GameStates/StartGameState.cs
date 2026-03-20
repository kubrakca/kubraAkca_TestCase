using UI;
using UnityEngine;

namespace Core.GameStates
{
    public class StartGameState : GameState
    {
        private StartScreen _startScreen;

        public override void Enter()
        {
            base.Enter();
            
            _startScreen = UIService.Show<StartScreen>();

            if (_startScreen != null)
            {
                _startScreen.OnPlayRequested += HandlePlayRequested;
            }
        }

        private void HandlePlayRequested()
        {
            Context.ChangeState<PlayingGameState>();
        }

        public override void Exit()
        {
            if (_startScreen != null)
            {
                _startScreen.OnPlayRequested -= HandlePlayRequested;
            }

            base.Exit();
        }
    }
}