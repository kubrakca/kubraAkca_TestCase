using UI;
using UnityEngine;
using Zenject;

namespace Core.GameStates
{
    /// <summary>Base for a single screen/phase of the game; Enter/Exit toggle this behaviour’s GameObject.</summary>
    public abstract class GameState : MonoBehaviour
    {
        #region Protected Fields

        protected GameStateFactory Context;

        #endregion

        #region Dependency Injection

        [Inject] protected IUIService UIService;

        #endregion

        #region Public Methods

        /// <summary>Called by <see cref="GameStateFactory"/> before the first transition.</summary>
        public void SetContext(GameStateFactory context) => Context = context;

        /// <summary>Activates this state object; override to subscribe events and start work.</summary>
        public virtual void Enter()
        {
            Debug.Log($"Enter: {GetType().Name}");
            gameObject.SetActive(true);
        }

        /// <summary>Deactivates this state object; override to unsubscribe and release resources.</summary>
        public virtual void Exit()
        {
            Debug.Log($"Exit: {GetType().Name}");
            gameObject.SetActive(false);
        }

        #endregion
    }
}
