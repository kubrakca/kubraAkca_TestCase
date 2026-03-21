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
        [SerializeField] private Color disabledColor;
        [SerializeField] private Color lockedColor;
        [SerializeField] private Color disableTextColor;

        public event Action<int> OnLevelSelected;
        private int _dataIndex;

        public void Setup(int displayNumber, int dataIndex, LevelStatus status)
        {
            _dataIndex = dataIndex;
            levelText.text = displayNumber.ToString();

            button.interactable = status != LevelStatus.Locked;

            switch (status)
            {
                case LevelStatus.Locked:
                    backgroundImage.color = lockedColor;
                    transform.localScale = Vector3.one;
                    break;

                case LevelStatus.Active:
                    backgroundImage.color = Color.white;
                    transform.localScale = Vector3.one * 1.3f;
                    break;

                case LevelStatus.Completed:
                    backgroundImage.color = disabledColor;
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
