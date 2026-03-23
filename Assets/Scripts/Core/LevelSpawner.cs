using System;
using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects;
using ScriptableObjects.Scripts;
using UnityEngine;
using Zenject;

namespace Core
{
    /// <summary>
    /// Builds the level root: background grid visuals, instances stars/gates/obstacles from <see cref="LevelData"/>, fits orthographic camera.
    /// </summary>
    public class LevelSpawner : MonoBehaviour
    {
        #region SerializeField

        [Header("Camera fit")]
        [Tooltip("Extra world units beyond grid bounds (cells + border gates).")]
        [SerializeField] private float cameraFitMarginWorld = 0.55f;

        #endregion

        #region Public Fields

        public event Action OnAllStarsCollected;

        #endregion

        #region Private Fields

        private Transform _gridRoot;
        private GridVisualizer _gridVisualizer;
        private readonly List<StarElement> _activeStars = new();
        private readonly List<GateElement> _activeGates = new();
        private readonly List<ObstacleElement> _activeObstacles = new();

        #endregion

        #region Dependency Injection

        [Inject] private LevelDatabase _levelDatabase;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _gridVisualizer = gameObject.AddComponent<GridVisualizer>();
        }

        #endregion

        #region Public Methods

        /// <summary>Clears previous level, draws grid, spawns entities, recenters and sizes main camera to bounds.</summary>
        public void SpawnLevel(LevelData data)
        {
            ClearLevel();

            _gridRoot = new GameObject($"Level_{data.levelNumber}_Grid").transform;

            int offsetX = data.gridSize.x / 2;
            int offsetY = data.gridSize.y / 2;
            var gridMin = new Vector2Int(-offsetX, -offsetY);
            var gridMax = new Vector2Int(data.gridSize.x - offsetX - 1, data.gridSize.y - offsetY - 1);
            _gridVisualizer.GenerateGrid(gridMin, gridMax, _gridRoot);

            float centerX = (gridMin.x + gridMax.x) / 2f;
            float centerY = (gridMin.y + gridMax.y) / 2f;
            FitCameraToGrid(gridMin, gridMax, centerX, centerY);

            if (data.stars != null)
            {
                foreach (var starData in data.stars)
                {
                    var star = Instantiate(_levelDatabase.starPrefab, _gridRoot);
                    star.transform.position = new Vector3(starData.gridPosition.x - offsetX, starData.gridPosition.y - offsetY, 0);
                    star.color = starData.color;
                    star.ApplyColor();
                    _activeStars.Add(star);
                }
            }

            if (data.gates != null)
            {
                foreach (var gateData in data.gates)
                {
                    var gate = Instantiate(_levelDatabase.gatePrefab, _gridRoot);
                    gate.transform.position = new Vector3(gateData.gridPosition.x - offsetX, gateData.gridPosition.y - offsetY, 0);
                    gate.transform.rotation = Quaternion.identity;
                    if (gateData.size.x > 0 && gateData.size.y > 0)
                        gate.transform.localScale = new Vector3(gateData.size.x, gateData.size.y, 1);
                    if (gateData.orientation == Enums.GateOrientation.Horizontal)
                        gate.transform.rotation = Quaternion.Euler(0, 0, 90);
                    gate.color = gateData.color;
                    gate.ApplyColor();
                    _activeGates.Add(gate);
                }
            }

            if (data.obstacles != null)
            {
                foreach (var obstacleData in data.obstacles)
                {
                    var obstacle = Instantiate(_levelDatabase.obstaclePrefab, _gridRoot);
                    obstacle.transform.position = new Vector3(obstacleData.gridPosition.x - offsetX, obstacleData.gridPosition.y - offsetY, 0);
                    obstacle.transform.localScale = new Vector3(obstacleData.size.x, obstacleData.size.y, 1);
                    obstacle.ApplyColor();
                    _activeObstacles.Add(obstacle);
                }
            }
        }

        /// <summary>Destroys star instance; raises <see cref="OnAllStarsCollected"/> when the last one is removed.</summary>
        public void RemoveStar(StarElement star)
        {
            if (_activeStars.Remove(star))
            {
                Destroy(star.gameObject);

                if (_activeStars.Count == 0)
                {
                    OnAllStarsCollected?.Invoke();
                }
            }
        }

        public List<StarElement> GetActiveStars() => _activeStars;
        public List<GateElement> GetActiveGates() => _activeGates;
        public List<ObstacleElement> GetActiveObstacles() => _activeObstacles;

        /// <summary>Removes the whole level hierarchy and clears cached element lists.</summary>
        public void ClearLevel()
        {
            _activeStars.Clear();
            _activeGates.Clear();
            _activeObstacles.Clear();

            if (_gridRoot != null)
            {
                Destroy(_gridRoot.gameObject);
                _gridRoot = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>Ensures the full grid (plus margin) is visible for current screen aspect.</summary>
        private void FitCameraToGrid(Vector2Int gridMin, Vector2Int gridMax, float centerX, float centerY)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("[LevelSpawner] Camera.main is null. Tag your gameplay camera with MainCamera.");
                return;
            }

            cam.orthographic = true;
            cam.transform.position = new Vector3(centerX, centerY, cam.transform.position.z);

            float worldWidth = gridMax.x - gridMin.x + 1f;
            float worldHeight = gridMax.y - gridMin.y + 1f;
            float halfW = worldWidth * 0.5f + cameraFitMarginWorld;
            float halfH = worldHeight * 0.5f + cameraFitMarginWorld;

            float aspect = cam.pixelHeight > 0
                ? (float)cam.pixelWidth / cam.pixelHeight
                : Mathf.Max(0.0001f, cam.aspect);
            float sizeForHeight = halfH;
            float sizeForWidth = halfW / aspect;
            cam.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);
        }

        #endregion
    }
}
