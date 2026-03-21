using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Enums;

namespace UI
{
    public class LevelButtonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button button;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color completedColor = Color.gray;
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        public event Action<int> OnLevelSelected;
        
        private int _dataIndex;
        private LevelStatus _status;
        private bool _isSelected;

        public void Setup(int displayNumber, int dataIndex, LevelStatus status)
        {
            _dataIndex = dataIndex;
            _status = status;
            _isSelected = false;
            levelText.text = displayNumber.ToString();
            button.interactable = status != LevelStatus.Locked;
            UpdateVisual();
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_isSelected && _status != LevelStatus.Locked)
            {
                backgroundImage.color = selectedColor;
                transform.localScale = Vector3.one * 1.3f;
                return;
            }

            switch (_status)
            {
                case LevelStatus.Locked:
                    backgroundImage.color = lockedColor;
                    transform.localScale = Vector3.one;
                    break;
                case LevelStatus.Active:
                    backgroundImage.color = activeColor;
                    transform.localScale = Vector3.one * 1.1f;
                    break;
                case LevelStatus.Completed:
                    backgroundImage.color = completedColor;
                    transform.localScale = Vector3.one;
                    break;
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(() => OnLevelSelected?.Invoke(_dataIndex));
        }
    }
}
