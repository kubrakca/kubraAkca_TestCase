using Enums;
using UnityEngine;

namespace LevelEditor
{
    public class StarElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public ColorType color;

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
    }
}
