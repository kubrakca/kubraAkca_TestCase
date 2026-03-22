using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsScreen : UIView
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button homeButton;

        public event Action OnResumeClicked;
        public event Action OnHomeClicked;

        private void Awake()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
            if (homeButton != null)
                homeButton.onClick.AddListener(() => OnHomeClicked?.Invoke());
        }
    }
}
