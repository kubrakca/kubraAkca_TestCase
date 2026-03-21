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
            spriteRenderer.color = color.ToColor();
        }
    }
}
