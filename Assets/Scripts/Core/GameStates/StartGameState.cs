using UI;
using UnityEngine;

namespace Core.GameStates
{
    public class StartGameState : GameState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Start Game State");
            UIService.Show<StartScreen>();

        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit Game State");
            ToNextState();
        }

        private void ToNextState()
        {
            Context.ChangeState<PlayingGameState>();
        }
    }
}