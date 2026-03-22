using System;
using System.Collections;
using DG.Tweening;
using ScriptableObjects.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameScreen : UIView
    {
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

        public event Action OnTimerExpired;
        public event Action OnPauseClicked;

        private Coroutine _timerCoroutine;
        private float _remainingTime;
        private bool _isPlaying;
        private bool _isUrgent;
        private Color _defaultTimerColor;

        private void Awake()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(() => OnPauseClicked?.Invoke());

            _defaultTimerColor = timerText.color;
        }

        public void Initialize(LevelData data)
        {
            levelIndicatorText.text = $"{data.levelNumber}";
            _remainingTime = data.timeLimit;
            _isPlaying = true;
            _isUrgent = false;

            timerText.color = _defaultTimerColor;
            timerText.transform.localScale = Vector3.one;

            _timerCoroutine = StartCoroutine(TimerCountdown());
        }

        private IEnumerator TimerCountdown()
        {
            while (_remainingTime > 0 && _isPlaying)
            {
                _remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();

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
                UpdateTimerDisplay();
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

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(_remainingTime / 60f);
            int seconds = Mathf.FloorToInt(_remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        public void PauseTimer()
        {
            _isPlaying = false;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        public void ResumeTimer()
        {
            _isPlaying = true;
            _timerCoroutine = StartCoroutine(TimerCountdown());
        }

        public void StopTimer()
        {
            _isPlaying = false;
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        public override void Hide()
        {
            StopTimer();
            timerText.DOKill();
            timerText.transform.DOKill();
            base.Hide();
        }
    }
}
