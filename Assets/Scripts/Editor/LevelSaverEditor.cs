using System.Collections.Generic;
using System.IO;
using Enums;
using UnityEditor;
using UnityEngine;
using LevelEditor;
using ScriptableObjects;
using ScriptableObjects.Scripts;

namespace Editor
{
    public class LevelSaverEditor : EditorWindow
    {
        private LevelData _targetLevelData;
        private LevelDatabase _levelDatabase;

        private const string LevelsPath = "Assets/ScriptableObjects/Levels";
        private const string DatabasePath = "Assets/ScriptableObjects/Levels/LevelDatabase.asset";

        [MenuItem("Tools/Level System/Level Editor & Saver")]
        public static void ShowWindow() => GetWindow<LevelSaverEditor>("Level Editor");

        private void OnEnable()
        {
            _levelDatabase = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DatabasePath);
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Editor Settings", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            _levelDatabase = (LevelDatabase)EditorGUILayout.ObjectField("Level Database", _levelDatabase, typeof(LevelDatabase), false);

            EditorGUILayout.Space(10);
            _targetLevelData = (LevelData)EditorGUILayout.ObjectField("Target Level SO", _targetLevelData, typeof(LevelData), false);

            if (_targetLevelData != null)
            {
                EditorGUILayout.Space(10);
                _targetLevelData.levelNumber = EditorGUILayout.IntField("Level Number", _targetLevelData.levelNumber);
                _targetLevelData.timeLimit = EditorGUILayout.FloatField("Level Timer (Seconds)", _targetLevelData.timeLimit);
                _targetLevelData.gridSize = EditorGUILayout.Vector2IntField("Grid Size", _targetLevelData.gridSize);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Elements in Scene", EditorStyles.boldLabel);
                int starCount = FindObjectsByType<StarElement>(FindObjectsSortMode.None).Length;
                int gateCount = FindObjectsByType<GateElement>(FindObjectsSortMode.None).Length;
                int obsCount = FindObjectsByType<ObstacleElement>(FindObjectsSortMode.None).Length;
                EditorGUILayout.LabelField($"Stars: {starCount}  |  Gates: {gateCount}  |  Obstacles: {obsCount}");

                EditorGUILayout.Space(20);

                GUI.color = Color.green;
                if (GUILayout.Button("SAVE SCENE TO SO", GUILayout.Height(40)))
                {
                    SaveLevel();
                }

                GUI.color = Color.cyan;
                if (GUILayout.Button("LOAD SO TO SCENE", GUILayout.Height(30)))
                {
                    LoadLevelToScene();
                }
                GUI.color = Color.white;
            }

            EditorGUILayout.Space(30);
            GUI.color = Color.yellow;
            if (GUILayout.Button("CREATE NEW LEVEL", GUILayout.Height(40)))
            {
                CreateNewLevel();
            }
            GUI.color = Color.white;
        }

        private void SaveLevel()
        {
            if (_targetLevelData == null) return;

            _targetLevelData.stars = new List<StarData>();
            _targetLevelData.gates = new List<GateData>();
            _targetLevelData.obstacles = new List<ObstacleData>();

            foreach (var star in FindObjectsByType<StarElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.stars.Add(new StarData
                {
                    gridPosition = Vector2Int.RoundToInt(star.transform.position),
                    color = star.color
                });
            }

            foreach (var gate in FindObjectsByType<GateElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.gates.Add(new GateData
                {
                    gridPosition = Vector2Int.RoundToInt(gate.transform.position),
                    color = gate.color,
                    size = Vector2Int.RoundToInt(gate.transform.localScale)
                });
            }

            foreach (var obs in FindObjectsByType<ObstacleElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.obstacles.Add(new ObstacleData
                {
                    gridPosition = Vector2Int.RoundToInt(obs.transform.position),
                    size = Vector2Int.RoundToInt(obs.transform.localScale)
                });
            }

            EditorUtility.SetDirty(_targetLevelData);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=green>{_targetLevelData.name} Updated!</color>");
        }

        private void LoadLevelToScene()
        {
            if (_targetLevelData == null) return;
            if (_levelDatabase == null)
            {
                Debug.LogError("LevelDatabase is not assigned! Please assign it in the Level Editor window.");
                return;
            }

            var allElements = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var element in allElements)
            {
                if (element is StarElement || element is GateElement || element is ObstacleElement)
                {
                    DestroyImmediate(element.gameObject);
                }
            }

            var editorGrid = GameObject.Find("EditorGrid");
            if (editorGrid != null) DestroyImmediate(editorGrid);

            CreateEditorGrid(_targetLevelData.gridSize);

            if (_targetLevelData.stars != null)
            {
                foreach (var starData in _targetLevelData.stars)
                {
                    var star = (StarElement)PrefabUtility.InstantiatePrefab(_levelDatabase.starPrefab);
                    star.transform.position = new Vector3(starData.gridPosition.x, starData.gridPosition.y, 0);
                    star.color = starData.color;
                    star.ApplyColor();
                    star.gameObject.name = $"Star_{starData.color}";
                }
            }

            if (_targetLevelData.gates != null)
            {
                foreach (var gateData in _targetLevelData.gates)
                {
                    var gate = (GateElement)PrefabUtility.InstantiatePrefab(_levelDatabase.gatePrefab);
                    gate.transform.position = new Vector3(gateData.gridPosition.x, gateData.gridPosition.y, 0);
                    if (gateData.size.x > 0 && gateData.size.y > 0)
                        gate.transform.localScale = new Vector3(gateData.size.x, gateData.size.y, 1);
                    gate.color = gateData.color;
                    
                    gate.ApplyColor();
                    gate.gameObject.name = $"Gate_{gateData.color}";
                }
            }

            if (_targetLevelData.obstacles != null)
            {
                foreach (var obstacleData in _targetLevelData.obstacles)
                {
                    var obstacle = (ObstacleElement)PrefabUtility.InstantiatePrefab(_levelDatabase.obstaclePrefab);
                    obstacle.transform.position = new Vector3(obstacleData.gridPosition.x, obstacleData.gridPosition.y, 0);
                    obstacle.transform.localScale = new Vector3(obstacleData.size.x, obstacleData.size.y, 1);
                    obstacle.ApplyColor();
                    obstacle.gameObject.name = "Obstacle";
                }
            }

            Debug.Log($"<color=cyan>{_targetLevelData.name} loaded to scene!</color>");
        }

        private void CreateEditorGrid(Vector2Int gridSize)
        {
            var gridRoot = new GameObject("EditorGrid");
            int halfW = gridSize.x / 2;
            int halfH = gridSize.y / 2;
            int maxX = gridSize.x - halfW - 1;
            int maxY = gridSize.y - halfH - 1;

            var bg = new GameObject("Background");
            bg.transform.SetParent(gridRoot.transform);
            float centerX = ((-halfW) + maxX) / 2f;
            float centerY = ((-halfH) + maxY) / 2f;
            bg.transform.position = new Vector3(centerX, centerY, 0.1f);
            bg.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
            var bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = CreateSquareSprite();
            bgSr.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
            bgSr.sortingOrder = -10;

            for (int x = -halfW; x <= maxX; x++)
            {
                var line = new GameObject($"VLine_{x}");
                line.transform.SetParent(gridRoot.transform);
                line.transform.position = new Vector3(x + 0.5f, centerY, 0.05f);
                line.transform.localScale = new Vector3(0.03f, gridSize.y, 1);
                var sr = line.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.color = new Color(0.3f, 0.3f, 0.5f, 0.5f);
                sr.sortingOrder = -9;
            }

            for (int y = -halfH; y <= maxY; y++)
            {
                var line = new GameObject($"HLine_{y}");
                line.transform.SetParent(gridRoot.transform);
                line.transform.position = new Vector3(centerX, y + 0.5f, 0.05f);
                line.transform.localScale = new Vector3(gridSize.x, 0.03f, 1);
                var sr = line.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSquareSprite();
                sr.color = new Color(0.3f, 0.3f, 0.5f, 0.5f);
                sr.sortingOrder = -9;
            }
        }

        private static Sprite _editorSquare;

        private static Sprite CreateSquareSprite()
        {
            if (_editorSquare != null) return _editorSquare;

            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < 16; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            _editorSquare = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            return _editorSquare;
        }

        private void CreateNewLevel()
        {
            if (!Directory.Exists(LevelsPath))
            {
                Directory.CreateDirectory(LevelsPath);
                AssetDatabase.Refresh();
            }

            int nextLevelNumber = 1;
            if (_levelDatabase != null && _levelDatabase.levels != null)
            {
                nextLevelNumber = _levelDatabase.levels.Count + 1;
            }

            string assetPath = $"{LevelsPath}/Level{nextLevelNumber}.asset";
            while (AssetDatabase.LoadAssetAtPath<LevelData>(assetPath) != null)
            {
                nextLevelNumber++;
                assetPath = $"{LevelsPath}/Level{nextLevelNumber}.asset";
            }

            var newLevel = CreateInstance<LevelData>();
            newLevel.levelNumber = nextLevelNumber;
            newLevel.timeLimit = 60f;
            newLevel.gridSize = new Vector2Int(8, 8);
            newLevel.stars = new List<StarData>();
            newLevel.gates = new List<GateData>();
            newLevel.obstacles = new List<ObstacleData>();

            AssetDatabase.CreateAsset(newLevel, assetPath);

            if (_levelDatabase != null)
            {
                _levelDatabase.levels.Add(newLevel);
                EditorUtility.SetDirty(_levelDatabase);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _targetLevelData = newLevel;
            Debug.Log($"<color=yellow>Level {nextLevelNumber} created at {assetPath}</color>");
        }
    }
}
