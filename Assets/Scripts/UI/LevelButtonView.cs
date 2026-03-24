using System;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Single level entry on the start screen: label, lock/active/completed styling, selection highlight.</summary>
public class LevelButtonView : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [Tooltip("Scale when this level is the highlighted choice (tap selection).")]
    [SerializeField] private float selectionHighlightScale = 1.3f;
    [Tooltip("Scale for the current playable level when it is not the highlighted choice.")]
    [SerializeField] private float activeIdleScale = 1.1f;
    [Tooltip("Scale for locked and completed levels (idle).")]
    [SerializeField] private float notSelectedSize = 0.8f;

    #endregion

    #region Public Fields

    public event Action<int> OnLevelSelected;

    #endregion

    #region Private Fields

    private int _dataIndex;
    private LevelStatus _status;
    private bool _isSelected;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        button.onClick.AddListener(() => OnLevelSelected?.Invoke(_dataIndex));
    }

    #endregion

    #region Public Methods

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

    #endregion

    #region Private Methods

    private void UpdateVisual()
    {
        if (_isSelected && _status != LevelStatus.Locked)
        {
            backgroundImage.color = selectedColor;
            transform.localScale = Vector3.one * selectionHighlightScale;
            return;
        }

        switch (_status)
        {
            case LevelStatus.Locked:
                backgroundImage.color = lockedColor;
                transform.localScale = Vector3.one * notSelectedSize;
                break;
            case LevelStatus.Active:
                backgroundImage.color = activeColor;
                transform.localScale = Vector3.one * activeIdleScale;
                break;
            case LevelStatus.Completed:
                backgroundImage.color = activeColor;
                transform.localScale = Vector3.one * notSelectedSize;
                break;
        }
    }

    #endregion
}