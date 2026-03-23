using UnityEngine;

namespace LevelEditor
{
    public class ObstacleElement : MonoBehaviour
    {
        #region SerializeField

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Public Methods

        public void ApplyColor()
        {
            spriteRenderer.color = Color.gray;
        }

        #endregion
    }
}
