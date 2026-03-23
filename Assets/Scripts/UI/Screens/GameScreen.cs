using System;
using System.Collections;
using DG.Tweening;
using ScriptableObjects.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>In-game HUD: countdown timer with optional urgent styling, level label, pause signal.</summary>
    public class GameScreen : UIView
    {
        #region SerializeField

        [Header("UI References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text levelIndicatorText;
        [SerializeField] private Button pauseButton;

        [Header("Timer Urgent Animation")]
        [SerializeField] private Color urgentTimerColor = Color.red;
        [SerializeField] private float urgentThreshold = 15f;
        [SerializeField] private float urgentColorDuration = 0.3f;
        [SerializeField] private float urgentScaleUp = 1.4f;
        [SerializeField] private float urgentScaleUpDuration = 0.2f;
        [SerializeField] private Ease urgentScaleUpEase = Ease.OutBack;
        [SerializeField] private float urgentScaleDownDuration = 0.2f;
        [SerializeField] private Ease urgentScaleDownEase = Ease.InBack;

        #endregion

        #region Public Fields

        public event Action OnTimerExpired;
        public event Action OnPauseClicked;

        #endregion

        #region Private Fields

        private Coroutine _timerCoroutine;
        private float _remainingTime;
        private bool _isPlaying;
        private bool _isUrgent;
        private Color _defaultTimerColor;
        /// <summary>Displayed MM:SS tick; text refresh only when this whole-second value changes.</summary>
        private int _lastDisplayedTotalSeconds = int.MinValue;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(() => OnPauseClicked?.Invoke());

            _defaultTimerColor = timerText.color;
        }

        #endregion

        #region Public Methods

        /// <summary>Resets visuals and starts the countdown from <paramref name="data"/>.timeLimit.</summary>
        public void Initialize(LevelData data)
        {
            levelIndicatorText.SetText("{0}", data.levelNumber);
            _remainingTime = data.timeLimit;
            _isPlaying = true;
            _isUrgent = false;
            _lastDisplayedTotalSeconds = int.MinValue;

            timerText.color = _defaultTimerColor;
            timerText.transform.localScale = Vector3.one;

            RefreshTimerTextIfDirty(force: true);
            _timerCoroutine = StartCoroutine(TimerCountdown());
        }

        /// <summary>Freezes countdown and stops the coroutine (pause menu).</summary>
        public void PauseTimer()
        {
            _isPlaying = false;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        /// <summary>Continues countdown from stored remaining time.</summary>
        public void ResumeTimer()
        {
            _isPlaying = true;
            _lastDisplayedTotalSeconds = int.MinValue;
            RefreshTimerTextIfDirty(force: true);
            _timerCoroutine = StartCoroutine(TimerCountdown());
        }

        /// <summary>Ends timer updates permanently (leaving play state).</summary>
        public void StopTimer()
        {
            _isPlaying = false;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        /// <summary>Seconds left for scoring; clamped at zero.</summary>
        public float GetRemainingTime() => Mathf.Max(0f, _remainingTime);

        public override void Hide()
        {
            StopTimer();
            timerText.DOKill();
            timerText.transform.DOKill();
            base.Hide();
        }

        #endregion

        #region Private Methods

        /// <summary>Ticks each frame while playing; fires <see cref="OnTimerExpired"/> at zero.</summary>
        private IEnumerator TimerCountdown()
        {
            while (_remainingTime > 0 && _isPlaying)
            {
                _remainingTime -= Time.deltaTime;
                RefreshTimerTextIfDirty(force: false);

                if (!_isUrgent && _remainingTime <= urgentThreshold)
                {
                    _isUrgent = true;
                    PlayUrgentAnimation();
                }

                yield return null;
            }

            if (_isPlaying)
            {
                _isPlaying = false;
                _remainingTime = 0;
                RefreshTimerTextIfDirty(force: true);
                OnTimerExpired?.Invoke();
            }
        }

        private void PlayUrgentAnimation()
        {
            timerText.DOColor(urgentTimerColor, urgentColorDuration);
            timerText.transform
                .DOScale(urgentScaleUp, urgentScaleUpDuration)
                .SetEase(urgentScaleUpEase)
                .OnComplete(() =>
                    timerText.transform.DOScale(1f, urgentScaleDownDuration).SetEase(urgentScaleDownEase));
        }

        /// <summary>Updates TMP only when the floored second changes (or <paramref name="force"/>), reducing allocations.</summary>
        private void RefreshTimerTextIfDirty(bool force)
        {
            int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(_remainingTime));
            if (!force && totalSeconds == _lastDisplayedTotalSeconds)
                return;

            _lastDisplayedTotalSeconds = totalSeconds;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.SetText("{0:00}:{1:00}", minutes, seconds);
        }

        #endregion
    }
}
