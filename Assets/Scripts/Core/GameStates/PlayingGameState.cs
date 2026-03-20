using UnityEngine;

namespace Core.GameStates
{
    public class PlayingGameState : GameState
    {
        public override void Enter()
        {
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