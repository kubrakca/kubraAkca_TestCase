using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool; // Yeni pooling kütüphanesi

namespace UI
{
    public class StartScreen : UIView
    {
        [Header("References")]
        [SerializeField] private LevelButtonView buttonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Button startButton; 

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
                maxSize: 10
            );

            if (startButton != null)
            {
                startButton.onClick.AddListener(() => OnPlayRequested?.Invoke());
            }
        }

        public override void Show()
        {
            base.Show();
            Debug.Log("Start Screen Show");
            RefreshLevelButtons();
        }

        public override void Hide()
        {
            base.Hide();
            ClearButtons();
            Debug.Log("Start Screen Hide");
        }

        private void RefreshLevelButtons()
        {
            ClearButtons();

            for (int i = 1; i <= 5; i++)
            {
                LevelStatus status = i < 3 ? LevelStatus.Completed : (i == 3 ? LevelStatus.Active : LevelStatus.Locked);
                
                var btn = _pool.Get();
                btn.transform.SetParent(buttonContainer, false);
                btn.Setup(i, status);

                btn.OnLevelSelected += HandleLevelSelected;
                
                _activeButtons.Add(btn);
            }
        }

        private void HandleLevelSelected(int levelIndex)
        {
            Debug.Log($"Level {levelIndex} seçildi!");
            OnLevelSelected?.Invoke(levelIndex);
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