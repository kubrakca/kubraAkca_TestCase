using UnityEngine;

namespace Core
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        [SerializeField] private Color lineColor = new Color(0.3f, 0.3f, 0.5f, 0.5f);
        [SerializeField] private Color borderColor = new Color(0.4f, 0.4f, 0.6f, 0.8f);
        [SerializeField] private float lineThickness = 0.03f;
        [SerializeField] private float borderThickness = 0.08f;
        [SerializeField] private float cellPadding = 0.02f;

        public void GenerateGrid(Vector2Int gridSize, Transform parent)
        {
            float width = gridSize.x;
            float height = gridSize.y;
            float halfW = width / 2f;
            float halfH = height / 2f;

            CreateBackground(halfW, halfH, width, height, parent);
            CreateGridLines(gridSize, halfW, halfH, width, height, parent);
            CreateBorder(halfW, halfH, width, height, parent);
        }

        private void CreateBackground(float halfW, float halfH, float width, float height, Transform parent)
        {
            var bg = CreateSprite("GridBackground", parent);
            bg.transform.localPosition = new Vector3(0, 0, 0.1f);
            bg.transform.localScale = new Vector3(width + 0.2f, height + 0.2f, 1);
            bg.color = backgroundColor;
            bg.sortingOrder = -10;
        }

        private void CreateGridLines(Vector2Int gridSize, float halfW, float halfH, float width, float height, Transform parent)
        {
            for (int x = 1; x < gridSize.x; x++)
            {
                var line = CreateSprite($"VLine_{x}", parent);
                line.transform.localPosition = new Vector3(-halfW + x, 0, 0.05f);
                line.transform.localScale = new Vector3(lineThickness, height, 1);
                line.color = lineColor;
                line.sortingOrder = -9;
            }

            for (int y = 1; y < gridSize.y; y++)
            {
                var line = CreateSprite($"HLine_{y}", parent);
                line.transform.localPosition = new Vector3(0, -halfH + y, 0.05f);
                line.transform.localScale = new Vector3(width, lineThickness, 1);
                line.color = lineColor;
                line.sortingOrder = -9;
            }
        }

        private void CreateBorder(float halfW, float halfH, float width, float height, Transform parent)
        {
            var top = CreateSprite("BorderTop", parent);
            top.transform.localPosition = new Vector3(0, halfH, 0.02f);
            top.transform.localScale = new Vector3(width + borderThickness * 2, borderThickness, 1);
            top.color = borderColor;
            top.sortingOrder = -8;

            var bottom = CreateSprite("BorderBottom", parent);
            bottom.transform.localPosition = new Vector3(0, -halfH, 0.02f);
            bottom.transform.localScale = new Vector3(width + borderThickness * 2, borderThickness, 1);
            bottom.color = borderColor;
            bottom.sortingOrder = -8;

            var left = CreateSprite("BorderLeft", parent);
            left.transform.localPosition = new Vector3(-halfW, 0, 0.02f);
            left.transform.localScale = new Vector3(borderThickness, height + borderThickness * 2, 1);
            left.color = borderColor;
            left.sortingOrder = -8;

            var right = CreateSprite("BorderRight", parent);
            right.transform.localPosition = new Vector3(halfW, 0, 0.02f);
            right.transform.localScale = new Vector3(borderThickness, height + borderThickness * 2, 1);
            right.color = borderColor;
            right.sortingOrder = -8;
        }

        private SpriteRenderer CreateSprite(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateWhiteSquareSprite();
            return sr;
        }

        private static Sprite _cachedSquare;

        private static Sprite CreateWhiteSquareSprite()
        {
            if (_cachedSquare != null) return _cachedSquare;

            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < 16; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();

            _cachedSquare = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            return _cachedSquare;
        }
    }
}
