using Enums;
using UnityEngine;

namespace LevelEditor
{
    public class GateElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public ColorType color;
        public Vector2 exitDir;

        public void ApplyColor()
        {
            spriteRenderer.color = color.ToColor();
        }
    }
}
