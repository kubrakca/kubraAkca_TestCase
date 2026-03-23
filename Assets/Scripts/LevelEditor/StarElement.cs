using Enums;
using UnityEngine;

namespace LevelEditor
{
    public class StarElement : MonoBehaviour
    {
        #region SerializeField

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Public Fields

        public ColorType color;

        #endregion

        #region Public Methods

        public void ApplyColor()
        {
            spriteRenderer.color = color switch
            {
                ColorType.Red => Color.red,
                ColorType.Blue => Color.blue,
                ColorType.Green => Color.green,
                ColorType.Yellow => Color.yellow,
                _ => Color.white
            };
        }

        #endregion
    }
}
