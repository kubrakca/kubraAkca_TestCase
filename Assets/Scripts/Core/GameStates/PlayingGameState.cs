using UI;
using UnityEngine;

namespace Core.GameStates
{
    public class PlayingGameState : GameState
    {
        public override void Enter()
        {
            UIService.Show<GameScreen>();
            base.Enter();
            Debug.Log("Playing Game State");
            
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit Game State");
            ToNextState();
        }
        private void ToNextState()
        {
            Context.ChangeState<EndGameState>();
        }
    }
}