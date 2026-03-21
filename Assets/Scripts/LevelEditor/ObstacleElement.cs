using UnityEngine;

namespace LevelEditor
{
    public class ObstacleElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void ApplyColor()
        {
            spriteRenderer.color = Color.gray;
        }
    }
}
