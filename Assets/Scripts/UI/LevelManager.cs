using Enums;
using ScriptableObjects;
using ScriptableObjects.Scripts;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelManager : MonoBehaviour, ILevelService
    {
        [Inject] private LevelDatabase _levelDatabase;

        private const string CurrentLevelKey = "CurrentLevel";
        private const string LevelStatusKeyPrefix = "Level_Status_";

        public int LevelCount => _levelDatabase.LevelCount;
        public int CurrentLevelIndex { get; set; }

        private void Start()
        {
            LoadProgress();
        }

        public LevelData GetLevelData(int index)
        {
            return _levelDatabase.GetLevel(index);
        }

        public LevelStatus GetLevelStatus(int index)
        {
            int saved = PlayerPrefs.GetInt(LevelStatusKeyPrefix + index, index == 0 ? (int)LevelStatus.Active : (int)LevelStatus.Locked);
            return (LevelStatus)saved;
        }

        public void CompleteLevel(int index)
        {
            PlayerPrefs.SetInt(LevelStatusKeyPrefix + index, (int)LevelStatus.Completed);

            int nextIndex = index + 1;
            if (nextIndex < LevelCount)
            {
                int nextStatus = PlayerPrefs.GetInt(LevelStatusKeyPrefix + nextIndex, (int)LevelStatus.Locked);
                if (nextStatus == (int)LevelStatus.Locked)
                {
                    PlayerPrefs.SetInt(LevelStatusKeyPrefix + nextIndex, (int)LevelStatus.Active);
                }
            }

            CurrentLevelIndex = nextIndex < LevelCount ? nextIndex : index;
            SaveProgress();
        }

        public void SaveProgress()
        {
            PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevelIndex);
            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            CurrentLevelIndex = PlayerPrefs.GetInt(CurrentLevelKey, 0);
        }
    }
}
