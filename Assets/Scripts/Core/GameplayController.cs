using System;
using System.Collections;
using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects.Scripts;
using UnityEngine;
using DG.Tweening;
using Zenject;

namespace Core
{
    /// <summary>
    /// Puzzle input and rules: pick a star by tap, swipe to move or match a gate; tracks grid occupancy and win when no stars remain.
    /// </summary>
    public class GameplayController : MonoBehaviour
    {
        #region SerializeField

        [SerializeField] private float moveSpeed = 0.2f;
        [SerializeField] private float swipeThreshold = 30f;

        [Header("Star — gate match")]
        [SerializeField] private float gateHitScaleDownDuration = 0.25f;
        [SerializeField] private Ease gateHitScaleDownEase = Ease.InBack;

        #endregion

        #region Public Fields
        public event Action OnAllStarsMatched;
        #endregion
        
        #region Private Fields
        private readonly Dictionary<Vector2Int, StarElement> _starPositions = new();
        private readonly Dictionary<Vector2Int, GateElement> _gateBorderPositions = new();
        private readonly HashSet<Vector2Int> _obstaclePositions = new();

        private Camera _mainCamera;
        private Coroutine _inputCoroutine;
        private bool _isMoving;

        private Vector2Int _gridMin;
        private Vector2Int _gridMax;
        #endregion
        
        #region Dependency Injection
        [Inject] private LevelSpawner _levelSpawner;
        #endregion

        #region Public Methods

        /// <summary>Caches camera, rebuilds logical grid from spawned entities, starts input handling.</summary>
        public void Initialize(LevelData data)
        {
            _mainCamera = Camera.main;
            _isMoving = false;

            int halfW = data.gridSize.x / 2;
            int halfH = data.gridSize.y / 2;
            _gridMin = new Vector2Int(-halfW, -halfH);
            _gridMax = new Vector2Int(data.gridSize.x - halfW - 1, data.gridSize.y - halfH - 1);

            BuildGridMap();

            Debug.Log($"Grid bounds: {_gridMin} to {_gridMax}");
            Debug.Log($"Gate border keys: {string.Join(", ", _gateBorderPositions.Keys)}");
            Debug.Log($"Star positions: {string.Join(", ", _starPositions.Keys)}");

            _inputCoroutine = StartCoroutine(InputLoop());
        }

        /// <summary>Stops swipe/tap processing (e.g. while pause menu is open).</summary>
        public void Deactivate()
        {
            if (_inputCoroutine != null)
            {
                StopCoroutine(_inputCoroutine);
                _inputCoroutine = null;
            }
        }

        /// <summary>Resumes input after <see cref="Deactivate"/>.</summary>
        public void Reactivate()
        {
            if (_inputCoroutine == null)
                _inputCoroutine = StartCoroutine(InputLoop());
        }
        #endregion
        
        #region Private Methods

        /// <summary>Fills star/obstacle cell maps and gate map (gates keyed on half-step border coordinates).</summary>
        private void BuildGridMap()
        {
            _starPositions.Clear();
            _gateBorderPositions.Clear();
            _obstaclePositions.Clear();

            foreach (var star in _levelSpawner.GetActiveStars())
            {
                var gridPos = Vector2Int.RoundToInt(star.transform.position);
                _starPositions[gridPos] = star;
            }

            foreach (var gate in _levelSpawner.GetActiveGates())
            {
                var wp = gate.transform.position;
                var key = new Vector2Int(Mathf.RoundToInt(wp.x * 2), Mathf.RoundToInt(wp.y * 2));
                _gateBorderPositions[key] = gate;
            }

            foreach (var obstacle in _levelSpawner.GetActiveObstacles())
            {
                var gridPos = Vector2Int.RoundToInt(obstacle.transform.position);
                _obstaclePositions.Add(gridPos);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>Waits for press on a star, then release; applies swipe as a grid move if strong enough.</summary>
        private IEnumerator InputLoop()
        {
            while (true)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !_isMoving);

                Vector3 screenPos = Input.mousePosition;
                screenPos.z = Mathf.Abs(_mainCamera.transform.position.z);
                Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
                Vector2Int gridPos = Vector2Int.RoundToInt(worldPos);

                if (!_starPositions.TryGetValue(gridPos, out var selectedStar))
                    continue;

                Vector2 startScreenPos = Input.mousePosition;

                yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

                Vector2 endScreenPos = Input.mousePosition;
                Vector2 swipeDelta = endScreenPos - startScreenPos;

                if (swipeDelta.magnitude < swipeThreshold)
                    continue;

                Vector2Int direction = GetSwipeDirection(swipeDelta);
                TryMoveStar(gridPos, direction);
            }
        }

        private Vector2Int GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        private bool IsInsideGrid(Vector2Int pos)
        {
            return pos.x >= _gridMin.x && pos.x <= _gridMax.x &&
                   pos.y >= _gridMin.y && pos.y <= _gridMax.y;
        }

        /// <summary>Resolves gate match (same color) or empty cell slide; blocks obstacles, other stars, and out-of-bounds.</summary>
        private void TryMoveStar(Vector2Int from, Vector2Int direction)
        {
            var gateKey = new Vector2Int(from.x * 2 + direction.x, from.y * 2 + direction.y);

            if (_gateBorderPositions.TryGetValue(gateKey, out var gate))
            {
                var star = _starPositions[from];
                if (star.color == gate.color)
                {
                    _isMoving = true;
                    _starPositions.Remove(from);

                    Vector3 gateWorld = gate.transform.position;
                    star.transform.DOMove(gateWorld, moveSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        star.transform
                            .DOScale(0f, gateHitScaleDownDuration)
                            .SetEase(gateHitScaleDownEase)
                            .OnComplete(() =>
                            {
                                _levelSpawner.RemoveStar(star);
                                _isMoving = false;
                                CheckWinCondition();
                            });
                    });
                    return;
                }
                return;
            }

            Vector2Int target = from + direction;

            if (!IsInsideGrid(target))
                return;

            if (_obstaclePositions.Contains(target))
                return;

            if (_starPositions.ContainsKey(target))
                return;

            _isMoving = true;
            var movingStar = _starPositions[from];
            _starPositions.Remove(from);
            _starPositions[target] = movingStar;

            Vector3 targetPos = new Vector3(target.x, target.y, 0);
            movingStar.transform.DOMove(targetPos, moveSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _isMoving = false;
            });
        }

        private void CheckWinCondition()
        {
            if (_starPositions.Count == 0)
            {
                OnAllStarsMatched?.Invoke();
            }
        }
        #endregion
    }
}
