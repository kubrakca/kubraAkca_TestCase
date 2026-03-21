using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using Zenject;

namespace UI
{
    public class StartScreen : UIView
    {
        [Header("References")]
        [SerializeField] private LevelButtonView buttonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button startButton;

        [Inject] private ILevelService _levelService;

        public event Action<int> OnLevelSelected;
        public event Action OnPlayRequested;

        private IObjectPool<LevelButtonView> _pool;
        private readonly List<LevelButtonView> _activeButtons = new();

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
                startButton.onClick.AddListener(() => OnPlayRequested?.Invoke());
            }
        }

        public override void Show()
        {
            base.Show();
            RefreshLevelButtons();
        }

        public override void Hide()
        {
            base.Hide();
            ClearButtons();
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
                btn.OnLevelSelected += HandleLevelSelected;

                _activeButtons.Add(btn);
            }
        }

        private void HandleLevelSelected(int dataIndex)
        {
            LevelStatus status = _levelService.GetLevelStatus(dataIndex);
            if (status == LevelStatus.Locked) return;

            OnLevelSelected?.Invoke(dataIndex);
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
