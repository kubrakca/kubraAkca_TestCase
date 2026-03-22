using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndScreen : UIView
    {
        [Header("Content Panels")]
        [SerializeField] private GameObject completedUIContent;
        [SerializeField] private GameObject notCompletedUIContent;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button replayButton;

        public event Action OnContinueClicked;
        public event Action OnReplayClicked;

        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
            if (replayButton != null)
                replayButton.onClick.AddListener(() => OnReplayClicked?.Invoke());
        }

        public void Initialize(bool isCompleted)
        {
            if (completedUIContent != null)
                completedUIContent.SetActive(isCompleted);
            if (notCompletedUIContent != null)
                notCompletedUIContent.SetActive(!isCompleted);
        }
    }
}
