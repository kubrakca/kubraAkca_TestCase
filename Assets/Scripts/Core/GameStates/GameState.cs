using UnityEngine;

namespace Core.GameStates
{
    public abstract class GameState : MonoBehaviour
    {
        protected GameStateFactory Context;

        public void SetContext(GameStateFactory context)
        {
            Context = context;
        }

        public virtual void Enter()
        {
            Debug.Log($"Enter: {GetType().Name}");
            gameObject.SetActive(true);
        }

        public virtual void Exit()
        {
            Debug.Log($"Exit: {GetType().Name}");
            gameObject.SetActive(false);
        }
    }
}