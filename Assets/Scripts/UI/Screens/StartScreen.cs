using System;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Zenject;

namespace Screens
{
    public class StartScreen : UIView
    {
        [Header("References")]
        [SerializeField] private LevelButtonView buttonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button startButton;
        [SerializeField] private int levelButtonsStartSiblingIndex;

        [Header("Play Button Animation")]
        [SerializeField] private float playButtonScaleTarget = 1.1f;
        [SerializeField] private float playButtonScaleDuration = 0.6f;
        [SerializeField] private Ease playButtonEase = Ease.InOutSine;

        [Inject] private ILevelService _levelService;

        public event Action<int> OnLevelSelected;
        public event Action OnPlayRequested;

        private IObjectPool<LevelButtonView> _pool;
        private readonly List<LevelButtonView> _activeButtons = new();
        private int _selectedLevelIndex = -1;
        private Tween _playButtonTween;

        private void Awake()
        {
            _pool = new ObjectPool<LevelButtonView>(
                createFunc: () => Instantiate(buttonPrefab, buttonContainer),
                actionOnGet: (btn) => btn.gameObject.SetActive(true),
                actionOnRelease: (btn) => btn.gameObject.SetActive(false),
                actionOnDestroy: (btn) => Destroy(btn.gameObject),
                defaultCapacity: 5,
                maxSize: 20
            );

            if (startButton != null)
            {
                startButton.onClick.AddListener(HandlePlayButton);
            }
        }

        public override void Show()
        {
            base.Show();
            _selectedLevelIndex = _levelService.CurrentLevelIndex;
            RefreshLevelButtons();
            StartPlayButtonAnimation();
        }

        public override void Hide()
        {
            KillPlayButtonAnimation();
            base.Hide();
            ClearButtons();
        }

        private void StartPlayButtonAnimation()
        {
            if (startButton == null) return;
            startButton.transform.localScale = Vector3.one;
            _playButtonTween = startButton.transform
                .DOScale(playButtonScaleTarget, playButtonScaleDuration)
                .SetEase(playButtonEase)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void KillPlayButtonAnimation()
        {
            _playButtonTween?.Kill();
            _playButtonTween = null;
            if (startButton != null)
                startButton.transform.localScale = Vector3.one;
        }

        private void RefreshLevelButtons()
        {
            ClearButtons();

            for (int i = 0; i < _levelService.LevelCount; i++)
            {
                LevelStatus status = _levelService.GetLevelStatus(i);

                var btn = _pool.Get();
                btn.transform.SetParent(buttonContainer, false);
                btn.Setup(i + 1, i, status);
                btn.SetSelected(i == _selectedLevelIndex);
                btn.OnLevelSelected += HandleLevelSelected;

                _activeButtons.Add(btn);
            }

            int n = _activeButtons.Count;
            for (int i = n - 1; i >= 0; i--)
                _activeButtons[i].transform.SetSiblingIndex(levelButtonsStartSiblingIndex + (n - 1 - i));
        }

        private void HandleLevelSelected(int dataIndex)
        {
            LevelStatus status = _levelService.GetLevelStatus(dataIndex);
            if (status == LevelStatus.Locked) return;

            _selectedLevelIndex = dataIndex;

            for (int i = 0; i < _activeButtons.Count; i++)
            {
                _activeButtons[i].SetSelected(i == dataIndex);
            }

            OnLevelSelected?.Invoke(dataIndex);
        }

        private void HandlePlayButton()
        {
            if (_selectedLevelIndex < 0 || _selectedLevelIndex >= _levelService.LevelCount) return;

            LevelStatus status = _levelService.GetLevelStatus(_selectedLevelIndex);
            if (status == LevelStatus.Locked) return;

            OnPlayRequested?.Invoke();
        }

        private void ClearButtons()
        {
            foreach (var btn in _activeButtons)
            {
                btn.OnLevelSelected -= HandleLevelSelected;
                _pool.Release(btn);
            }
            _activeButtons.Clear();
        }
    }
}
