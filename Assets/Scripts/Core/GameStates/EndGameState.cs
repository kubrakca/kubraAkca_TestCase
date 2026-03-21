using UnityEngine;
using Zenject;
using UI;

namespace Core.GameStates
{
    public class EndGameState : GameState
    {
        [Inject] private ILevelService _levelService;

        public override void Enter()
        {
            base.Enter();
            _levelService.SaveProgress();
            Debug.Log("End Game State - Progress saved");

            ReturnToStart();
        }

        private void ReturnToStart()
        {
            Context.ChangeState<StartGameState>();
        }
    }
}
