using System.Collections.Generic;
using LevelEditor;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelSaverEditor : EditorWindow
    {
        private LevelData _targetLevelData;

        [MenuItem("Tools/Level Saver")]
        public static void ShowWindow() => GetWindow<LevelSaverEditor>("Level Saver");

        private void OnGUI()
        {
            GUILayout.Label("Level Save Settings", EditorStyles.boldLabel);
            _targetLevelData = (LevelData)EditorGUILayout.ObjectField("Target SO", _targetLevelData, typeof(LevelData), false);

            if (GUILayout.Button("SAVE SCENE TO SO", GUILayout.Height(40)))
            {
                SaveLevel();
            }
        }

        private void SaveLevel()
        {
            if (_targetLevelData == null) { Debug.LogError("Select a LevelData SO!"); return; }

            _targetLevelData.stars = new List<StarData>();
            _targetLevelData.gates = new List<GateData>();
            _targetLevelData.obstacles = new List<ObstacleData>();

            foreach (var star in FindObjectsByType<StarElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.stars.Add(new StarData { 
                    position = star.transform.position, 
                    color = star.color 
                });
            }

            foreach (var gate in FindObjectsByType<GateElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.gates.Add(new GateData { 
                    position = gate.transform.position, 
                    color = gate.color,
                    exitDirection = gate.exitDir
                });
            }

            foreach (var obs in FindObjectsByType<ObstacleElement>(FindObjectsSortMode.None))
            {
                _targetLevelData.obstacles.Add(new ObstacleData { 
                    position = obs.transform.position, 
                    scale = obs.transform.localScale 
                });
            }

            EditorUtility.SetDirty(_targetLevelData);
            AssetDatabase.SaveAssets();
            Debug.Log($"{_targetLevelData.name} saved");
        }
    }
}