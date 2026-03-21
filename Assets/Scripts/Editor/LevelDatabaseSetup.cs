using System.Collections.Generic;
using System.IO;
using Enums;
using ScriptableObjects;
using ScriptableObjects.Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class LevelDatabaseSetup
    {
        private const string LevelsPath = "Assets/ScriptableObjects/Levels";
        private const string DatabasePath = "Assets/ScriptableObjects/Levels/LevelDatabase.asset";

        [MenuItem("Tools/Level System/Generate 5 Levels + Database")]
        public static void GenerateLevelsAndDatabase()
        {
            if (!Directory.Exists(LevelsPath))
            {
                Directory.CreateDirectory(LevelsPath);
                AssetDatabase.Refresh();
            }

            var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DatabasePath);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<LevelDatabase>();
                AssetDatabase.CreateAsset(database, DatabasePath);
            }

            database.levels = new List<LevelData>();

            var level1 = LoadOrCreateLevel(1, 45f, new Vector2Int(6, 6), CreateLevel1Data());
            var level2 = LoadOrCreateLevel(2, 60f, new Vector2Int(7, 7), CreateLevel2Data());
            var level3 = LoadOrCreateLevel(3, 50f, new Vector2Int(8, 8), CreateLevel3Data());
            var level4 = LoadOrCreateLevel(4, 40f, new Vector2Int(8, 8), CreateLevel4Data());
            var level5 = LoadOrCreateLevel(5, 35f, new Vector2Int(9, 9), CreateLevel5Data());

            database.levels.Add(level1);
            database.levels.Add(level2);
            database.levels.Add(level3);
            database.levels.Add(level4);
            database.levels.Add(level5);

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green>5 levels + LevelDatabase created successfully!</color>");
        }

        private static LevelData LoadOrCreateLevel(int number, float timer, Vector2Int gridSize,
            (List<StarData> stars, List<GateData> gates, List<ObstacleData> obstacles) data)
        {
            string path = $"{LevelsPath}/Level{number}.asset";
            var level = AssetDatabase.LoadAssetAtPath<LevelData>(path);

            if (level == null)
            {
                level = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(level, path);
            }

            level.levelNumber = number;
            level.timeLimit = timer;
            level.gridSize = gridSize;
            level.stars = data.stars;
            level.gates = data.gates;
            level.obstacles = data.obstacles;

            EditorUtility.SetDirty(level);
            return level;
        }

        private static (List<StarData>, List<GateData>, List<ObstacleData>) CreateLevel1Data()
        {
            var stars = new List<StarData>
            {
                new() { gridPosition = new Vector2Int(1, 1), color = ColorType.Red },
                new() { gridPosition = new Vector2Int(4, 4), color = ColorType.Red },
            };
            var gates = new List<GateData>
            {
                new() { gridPosition = new Vector2Int(5, 1), color = ColorType.Red, exitDirection = Vector2Int.right },
            };
            var obstacles = new List<ObstacleData>
            {
                new() { gridPosition = new Vector2Int(3, 2), size = new Vector2Int(1, 2) },
            };
            return (stars, gates, obstacles);
        }

        private static (List<StarData>, List<GateData>, List<ObstacleData>) CreateLevel2Data()
        {
            var stars = new List<StarData>
            {
                new() { gridPosition = new Vector2Int(1, 1), color = ColorType.Blue },
                new() { gridPosition = new Vector2Int(5, 3), color = ColorType.Blue },
                new() { gridPosition = new Vector2Int(3, 5), color = ColorType.Red },
            };
            var gates = new List<GateData>
            {
                new() { gridPosition = new Vector2Int(6, 1), color = ColorType.Blue, exitDirection = Vector2Int.right },
                new() { gridPosition = new Vector2Int(6, 5), color = ColorType.Red, exitDirection = Vector2Int.right },
            };
            var obstacles = new List<ObstacleData>
            {
                new() { gridPosition = new Vector2Int(2, 3), size = new Vector2Int(2, 1) },
                new() { gridPosition = new Vector2Int(4, 1), size = new Vector2Int(1, 1) },
            };
            return (stars, gates, obstacles);
        }

        private static (List<StarData>, List<GateData>, List<ObstacleData>) CreateLevel3Data()
        {
            var stars = new List<StarData>
            {
                new() { gridPosition = new Vector2Int(1, 1), color = ColorType.Green },
                new() { gridPosition = new Vector2Int(6, 6), color = ColorType.Green },
                new() { gridPosition = new Vector2Int(3, 4), color = ColorType.Yellow },
                new() { gridPosition = new Vector2Int(5, 2), color = ColorType.Yellow },
            };
            var gates = new List<GateData>
            {
                new() { gridPosition = new Vector2Int(7, 1), color = ColorType.Green, exitDirection = Vector2Int.right },
                new() { gridPosition = new Vector2Int(7, 6), color = ColorType.Yellow, exitDirection = Vector2Int.up },
            };
            var obstacles = new List<ObstacleData>
            {
                new() { gridPosition = new Vector2Int(3, 2), size = new Vector2Int(1, 3) },
                new() { gridPosition = new Vector2Int(5, 5), size = new Vector2Int(2, 1) },
            };
            return (stars, gates, obstacles);
        }

        private static (List<StarData>, List<GateData>, List<ObstacleData>) CreateLevel4Data()
        {
            var stars = new List<StarData>
            {
                new() { gridPosition = new Vector2Int(1, 1), color = ColorType.Red },
                new() { gridPosition = new Vector2Int(6, 1), color = ColorType.Blue },
                new() { gridPosition = new Vector2Int(1, 6), color = ColorType.Green },
                new() { gridPosition = new Vector2Int(6, 6), color = ColorType.Red },
                new() { gridPosition = new Vector2Int(3, 3), color = ColorType.Blue },
            };
            var gates = new List<GateData>
            {
                new() { gridPosition = new Vector2Int(7, 3), color = ColorType.Red, exitDirection = Vector2Int.right },
                new() { gridPosition = new Vector2Int(0, 3), color = ColorType.Blue, exitDirection = Vector2Int.left },
                new() { gridPosition = new Vector2Int(3, 7), color = ColorType.Green, exitDirection = Vector2Int.up },
            };
            var obstacles = new List<ObstacleData>
            {
                new() { gridPosition = new Vector2Int(2, 2), size = new Vector2Int(1, 1) },
                new() { gridPosition = new Vector2Int(5, 4), size = new Vector2Int(1, 2) },
                new() { gridPosition = new Vector2Int(3, 5), size = new Vector2Int(2, 1) },
            };
            return (stars, gates, obstacles);
        }

        private static (List<StarData>, List<GateData>, List<ObstacleData>) CreateLevel5Data()
        {
            var stars = new List<StarData>
            {
                new() { gridPosition = new Vector2Int(1, 1), color = ColorType.Red },
                new() { gridPosition = new Vector2Int(7, 1), color = ColorType.Blue },
                new() { gridPosition = new Vector2Int(1, 7), color = ColorType.Green },
                new() { gridPosition = new Vector2Int(7, 7), color = ColorType.Yellow },
                new() { gridPosition = new Vector2Int(4, 4), color = ColorType.Red },
                new() { gridPosition = new Vector2Int(4, 1), color = ColorType.Green },
            };
            var gates = new List<GateData>
            {
                new() { gridPosition = new Vector2Int(8, 1), color = ColorType.Red, exitDirection = Vector2Int.right },
                new() { gridPosition = new Vector2Int(8, 4), color = ColorType.Blue, exitDirection = Vector2Int.right },
                new() { gridPosition = new Vector2Int(0, 7), color = ColorType.Green, exitDirection = Vector2Int.left },
                new() { gridPosition = new Vector2Int(4, 8), color = ColorType.Yellow, exitDirection = Vector2Int.up },
            };
            var obstacles = new List<ObstacleData>
            {
                new() { gridPosition = new Vector2Int(3, 3), size = new Vector2Int(1, 1) },
                new() { gridPosition = new Vector2Int(5, 5), size = new Vector2Int(1, 1) },
                new() { gridPosition = new Vector2Int(2, 6), size = new Vector2Int(2, 1) },
                new() { gridPosition = new Vector2Int(6, 2), size = new Vector2Int(1, 3) },
            };
            return (stars, gates, obstacles);
        }
    }
}
