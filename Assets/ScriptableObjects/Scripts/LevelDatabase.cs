using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects.Scripts;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level System/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        public List<LevelData> levels = new();

        [Header("Element Prefabs")]
        public StarElement starPrefab;
        public GateElement gatePrefab;
        public ObstacleElement obstaclePrefab;

        public int LevelCount => levels.Count;

        public LevelData GetLevel(int index)
        {
            if (index < 0 || index >= levels.Count) return null;
            return levels[index];
        }
    }
}
