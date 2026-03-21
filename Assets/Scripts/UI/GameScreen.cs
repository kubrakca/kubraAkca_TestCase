using System;
using System.Collections;
using ScriptableObjects.Scripts;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameScreen : UIView
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text levelIndicatorText;

        public event Action OnTimerExpired;

        private Coroutine _timerCoroutine;
        private float _remainingTime;
        private bool _isPlaying;

        public void Initialize(LevelData data)
        {
            levelIndicatorText.text = $"Level {data.levelNumber}";
            _remainingTime = data.timeLimit;
            _isPlaying = true;

            _timerCoroutine = StartCoroutine(TimerCountdown());
        }

        private IEnumerator TimerCountdown()
        {
            while (_remainingTime > 0 && _isPlaying)
            {
                _remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
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

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(_remainingTime / 60f);
            int seconds = Mathf.FloorToInt(_remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
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
            base.Hide();
        }
    }
}
