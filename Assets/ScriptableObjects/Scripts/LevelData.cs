using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Level System/Puzzle Level Data")]
    public class LevelData : ScriptableObject
    {
        public int levelNumber;
        public float timeLimit = 60f;
        
        [Header("Grid Settings")]
        public Vector2Int gridSize = new Vector2Int(3, 3);
        
        [Header("Level Layout")]
        public List<StarData> stars;
        public List<GateData> gates;
        public List<ObstacleData> obstacles;
    }

    [System.Serializable]
    public struct StarData {
        public Vector2 position;
        public ColorType color;
    }

    [System.Serializable]
    public struct GateData {
        public Vector2 position;
        public ColorType color;
        public Vector2 exitDirection; 
    }

    [System.Serializable]
    public struct ObstacleData {
        public Vector2 position;
        public Vector2 scale;
    }
}