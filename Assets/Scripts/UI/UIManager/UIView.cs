using UnityEngine;

namespace UI
{
    /// <summary>Base for full-screen (or modal) UI roots; default show/hide toggles the host GameObject.</summary>
    public abstract class UIView : MonoBehaviour 
    {
        #region Public Methods

        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);

        #endregion
    }
}
