using System.Collections.Generic;
using System.IO;
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
                    exitDirection = Vector2Int.RoundToInt(gate.exitDir)
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

            var allElements = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var element in allElements)
            {
                if (element is StarElement || element is GateElement || element is ObstacleElement)
                {
                    DestroyImmediate(element.gameObject);
                }
            }

            if (_targetLevelData.stars != null)
            {
                foreach (var starData in _targetLevelData.stars)
                {
                    var go = new GameObject($"Star_{starData.color}");
                    go.transform.position = new Vector3(starData.gridPosition.x, starData.gridPosition.y, 0);
                    var star = go.AddComponent<StarElement>();
                    star.color = starData.color;
                }
            }

            if (_targetLevelData.gates != null)
            {
                foreach (var gateData in _targetLevelData.gates)
                {
                    var go = new GameObject($"Gate_{gateData.color}");
                    go.transform.position = new Vector3(gateData.gridPosition.x, gateData.gridPosition.y, 0);
                    var gate = go.AddComponent<GateElement>();
                    gate.color = gateData.color;
                    gate.exitDir = gateData.exitDirection;
                }
            }

            if (_targetLevelData.obstacles != null)
            {
                foreach (var obstacleData in _targetLevelData.obstacles)
                {
                    var go = new GameObject("Obstacle");
                    go.transform.position = new Vector3(obstacleData.gridPosition.x, obstacleData.gridPosition.y, 0);
                    go.transform.localScale = new Vector3(obstacleData.size.x, obstacleData.size.y, 1);
                    go.AddComponent<ObstacleElement>();
                }
            }

            Debug.Log($"<color=cyan>{_targetLevelData.name} loaded to scene!</color>");
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
