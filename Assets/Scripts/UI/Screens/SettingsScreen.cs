using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>Pause overlay: resume continues play state; home returns to start flow.</summary>
    public class SettingsScreen : UIView
    {
        #region SerializeField

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button homeButton;

        #endregion

        #region Public Fields

        public event Action OnResumeClicked;
        public event Action OnHomeClicked;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
            if (homeButton != null)
                homeButton.onClick.AddListener(() => OnHomeClicked?.Invoke());
        }

        #endregion
    }
}
