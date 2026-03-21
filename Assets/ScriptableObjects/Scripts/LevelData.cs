using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Level System/Puzzle Level Data")]
    public class LevelData : ScriptableObject
    {
        public int levelNumber;
        public float timeLimit = 60f;
        
        public Vector2Int gridSize = new Vector2Int(8, 8); 

        public List<StarData> stars;
        public List<GateData> gates;
        public List<ObstacleData> obstacles;
    }

    [System.Serializable]
    public struct StarData {
        public Vector2Int gridPosition; 
        public ColorType color;
    }

    [System.Serializable]
    public struct GateData {
        public Vector2Int gridPosition;
        public ColorType color;
        public Vector2Int exitDirection;
    }

    [System.Serializable]
    public struct ObstacleData {
        public Vector2Int gridPosition;
        public Vector2Int size; 
    }
}