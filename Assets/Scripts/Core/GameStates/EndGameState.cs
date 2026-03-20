using UnityEngine;

namespace Core.GameStates
{
    public class EndGameState : GameState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("End Game State");
        }
    }
}