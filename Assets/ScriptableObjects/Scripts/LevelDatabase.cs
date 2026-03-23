using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects.Scripts;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>Ordered list of <see cref="LevelData"/> assets plus prefabs used when spawning level entities in play or editor.</summary>
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level System/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        public List<LevelData> levels = new();

        [Header("Element Prefabs")]
        public StarElement starPrefab;
        public GateElement gatePrefab;
        public ObstacleElement obstaclePrefab;

        public int LevelCount => levels.Count;

        /// <summary>Safe accessor; returns null when index is out of range.</summary>
        public LevelData GetLevel(int index)
        {
            if (index < 0 || index >= levels.Count) return null;
            return levels[index];
        }
    }
}
