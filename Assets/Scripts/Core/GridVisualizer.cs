using UnityEngine;

namespace Core
{
    /// <summary>Procedural board backdrop: tinted plane, per-cell line sprites, and outer border under gameplay entities.</summary>
    public class GridVisualizer : MonoBehaviour
    {
        #region SerializeField

        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        [SerializeField] private Color lineColor = new Color(0.3f, 0.3f, 0.5f, 0.5f);
        [SerializeField] private Color borderColor = new Color(0.4f, 0.4f, 0.6f, 0.8f);
        [SerializeField] private float lineThickness = 0.03f;
        [SerializeField] private float borderThickness = 0.08f;

        #endregion

        #region Private Fields

        private static Sprite _cachedSquare;

        #endregion

        #region Public Methods

        /// <summary>Creates child sprites under <paramref name="parent"/> covering inclusive cell range.</summary>
        public void GenerateGrid(Vector2Int gridMin, Vector2Int gridMax, Transform parent)
        {
            float left = gridMin.x - 0.5f;
            float right = gridMax.x + 0.5f;
            float bottom = gridMin.y - 0.5f;
            float top = gridMax.y + 0.5f;

            float width = right - left;
            float height = top - bottom;
            float centerX = (left + right) / 2f;
            float centerY = (bottom + top) / 2f;

            CreateBackground(centerX, centerY, width, height, parent);
            CreateGridLines(gridMin, gridMax, left, right, bottom, top, centerX, centerY, width, height, parent);
            CreateBorder(centerX, centerY, width, height, parent);
        }

        #endregion

        #region Private Methods

        private void CreateBackground(float cx, float cy, float w, float h, Transform parent)
        {
            var bg = CreateSprite("GridBackground", parent);
            bg.transform.localPosition = new Vector3(cx, cy, 0.1f);
            bg.transform.localScale = new Vector3(w, h, 1);
            bg.color = backgroundColor;
            bg.sortingOrder = -10;
        }

        private void CreateGridLines(Vector2Int gridMin, Vector2Int gridMax,
            float left, float right, float bottom, float top,
            float cx, float cy, float w, float h, Transform parent)
        {
            for (int x = gridMin.x; x <= gridMax.x; x++)
            {
                float lineX = x + 0.5f;
                var line = CreateSprite($"VLine_{x}", parent);
                line.transform.localPosition = new Vector3(lineX, cy, 0.05f);
                line.transform.localScale = new Vector3(lineThickness, h, 1);
                line.color = lineColor;
                line.sortingOrder = -9;
            }

            for (int y = gridMin.y; y <= gridMax.y; y++)
            {
                float lineY = y + 0.5f;
                var line = CreateSprite($"HLine_{y}", parent);
                line.transform.localPosition = new Vector3(cx, lineY, 0.05f);
                line.transform.localScale = new Vector3(w, lineThickness, 1);
                line.color = lineColor;
                line.sortingOrder = -9;
            }
        }

        private void CreateBorder(float cx, float cy, float w, float h, Transform parent)
        {
            var top = CreateSprite("BorderTop", parent);
            top.transform.localPosition = new Vector3(cx, cy + h / 2f, 0.02f);
            top.transform.localScale = new Vector3(w + borderThickness * 2, borderThickness, 1);
            top.color = borderColor;
            top.sortingOrder = -8;

            var bottom = CreateSprite("BorderBottom", parent);
            bottom.transform.localPosition = new Vector3(cx, cy - h / 2f, 0.02f);
            bottom.transform.localScale = new Vector3(w + borderThickness * 2, borderThickness, 1);
            bottom.color = borderColor;
            bottom.sortingOrder = -8;

            var left = CreateSprite("BorderLeft", parent);
            left.transform.localPosition = new Vector3(cx - w / 2f, cy, 0.02f);
            left.transform.localScale = new Vector3(borderThickness, h + borderThickness * 2, 1);
            left.color = borderColor;
            left.sortingOrder = -8;

            var right = CreateSprite("BorderRight", parent);
            right.transform.localPosition = new Vector3(cx + w / 2f, cy, 0.02f);
            right.transform.localScale = new Vector3(borderThickness, h + borderThickness * 2, 1);
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

        #endregion
    }
}
