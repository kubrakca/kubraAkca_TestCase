using System;
using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects;
using ScriptableObjects.Scripts;
using UnityEngine;
using Zenject;

namespace Core
{
    public class LevelSpawner : MonoBehaviour
    {
        [Inject] private LevelDatabase _levelDatabase;

        public event Action OnAllStarsCollected;

        private Transform _gridRoot;
        private GridVisualizer _gridVisualizer;
        private readonly List<StarElement> _activeStars = new();
        private readonly List<GateElement> _activeGates = new();
        private readonly List<ObstacleElement> _activeObstacles = new();

        private void Awake()
        {
            _gridVisualizer = gameObject.AddComponent<GridVisualizer>();
        }

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
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(centerX, centerY, cam.transform.position.z);

                float gridWidth = data.gridSize.x + 1f;
                float gridHeight = data.gridSize.y + 1f;
                float screenAspect = (float)Screen.width / Screen.height;
                float sizeForHeight = gridHeight / 2f;
                float sizeForWidth = gridWidth / (2f * screenAspect);
                cam.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth) + 1f;
            }

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
    }
}
