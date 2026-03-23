using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>Win/lose summary, animated star rating on success, continue to menu or replay current level.</summary>
    public class EndScreen : UIView
    {
        #region SerializeField

        [Header("Content Panels")]
        [SerializeField] private GameObject completedUIContent;
        [SerializeField] private GameObject notCompletedUIContent;

        [Header("Completed — Star Rating")]
        [SerializeField] private List<Image> completedStarImages;
        [SerializeField] private Color lockedStarColor = new Color(0.35f, 0.35f, 0.35f, 0.65f);
        [SerializeField] private float starPopDuration = 0.35f;
        [SerializeField] private Ease starPopEase = Ease.OutBack;
        [SerializeField] private float starStaggerDelay = 0.12f;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button replayButton;

        #endregion

        #region Public Fields

        public event Action OnContinueClicked;
        public event Action OnReplayClicked;

        #endregion

        #region Private Fields

        private readonly List<Color> _starOriginalColors = new();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
            if (replayButton != null)
                replayButton.onClick.AddListener(() => OnReplayClicked?.Invoke());

            CacheStarOriginalColors();
        }

        #endregion

        #region Public Methods

        /// <summary>1–3 stars from how much of <paramref name="timeLimit"/> was consumed (faster = more stars).</summary>
        public static int ComputeStarRating(float remainingTime, float timeLimit)
        {
            if (timeLimit <= 0.001f) return 3;

            float timeUsed = timeLimit - Mathf.Clamp(remainingTime, 0f, timeLimit);
            float ratioUsed = timeUsed / timeLimit;

            if (ratioUsed <= 1f / 3f) return 3;
            if (ratioUsed <= 2f / 3f) return 2;
            return 1;
        }

        /// <summary>Shows win or lose panel; on win, reveals <paramref name="starRating"/> filled stars with staggered tween.</summary>
        public void Initialize(bool isCompleted, int starRating = 0)
        {
            KillStarAnimation();

            ApplyCompletedPanelsExclusive(isCompleted);

            if (isCompleted)
                PlayStarReveal(starRating);
            else
                ResetStarsVisual();
        }

        public override void Hide()
        {
            KillStarAnimation();
            base.Hide();
        }

        #endregion

        #region Private Methods

        private void CacheStarOriginalColors()
        {
            _starOriginalColors.Clear();
            if (completedStarImages == null) return;

            foreach (var img in completedStarImages)
                _starOriginalColors.Add(img != null ? img.color : Color.white);
        }

        private void ApplyCompletedPanelsExclusive(bool showCompleted)
        {
            if (completedUIContent != null)
                completedUIContent.SetActive(false);
            if (notCompletedUIContent != null)
                notCompletedUIContent.SetActive(false);

            if (showCompleted)
            {
                if (completedUIContent != null)
                    completedUIContent.SetActive(true);
            }
            else
            {
                if (notCompletedUIContent != null)
                    notCompletedUIContent.SetActive(true);
            }
        }

        private void PlayStarReveal(int earnedStars)
        {
            if (completedStarImages == null || completedStarImages.Count == 0) return;

            int count = completedStarImages.Count;
            int earned = Mathf.Clamp(earnedStars, 0, count);

            for (int i = 0; i < count; i++)
            {
                var img = completedStarImages[i];
                if (img == null) continue;

                bool isEarned = i < earned;
                img.color = isEarned
                    ? (i < _starOriginalColors.Count ? _starOriginalColors[i] : Color.white)
                    : lockedStarColor;

                img.transform.localScale = Vector3.zero;
                img.transform
                    .DOScale(1f, starPopDuration)
                    .SetEase(starPopEase)
                    .SetDelay(i * starStaggerDelay);
            }
        }

        private void ResetStarsVisual()
        {
            if (completedStarImages == null) return;

            for (int i = 0; i < completedStarImages.Count; i++)
            {
                var img = completedStarImages[i];
                if (img == null) continue;

                img.transform.DOKill();
                img.transform.localScale = Vector3.one;
                img.color = i < _starOriginalColors.Count ? _starOriginalColors[i] : Color.white;
            }
        }

        private void KillStarAnimation()
        {
            if (completedStarImages == null) return;

            foreach (var img in completedStarImages)
            {
                if (img == null) continue;
                img.transform.DOKill();
            }
        }

        #endregion
    }
}
