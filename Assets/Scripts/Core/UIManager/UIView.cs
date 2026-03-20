using UnityEngine;

namespace UI
{
    public abstract class UIView : MonoBehaviour 
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
    }
}