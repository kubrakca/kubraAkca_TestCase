using Enums;
using ScriptableObjects;
using ScriptableObjects.Scripts;
using UnityEngine;
using Zenject;

namespace UI
{
    /// <summary>Level progression backed by <see cref="PlayerPrefs"/> and data from <see cref="LevelDatabase"/>.</summary>
    public class LevelManager : MonoBehaviour, ILevelService
    {
        #region Private Fields

        private const string CurrentLevelKey = "CurrentLevel";
        private const string LevelStatusKeyPrefix = "Level_Status_";

        #endregion

        #region Dependency Injection

        [Inject] private LevelDatabase _levelDatabase;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            LoadProgress();
        }

        #endregion

        #region Public Fields

        public int LevelCount => _levelDatabase.LevelCount;
        public int CurrentLevelIndex { get; set; }

        #endregion

        #region Public Methods

        /// <summary>Scriptable level definition at index, or null if missing.</summary>
        public LevelData GetLevelData(int index)
        {
            return _levelDatabase.GetLevel(index);
        }

        /// <summary>Locked / active / completed flag persisted per level index.</summary>
        public LevelStatus GetLevelStatus(int index)
        {
            int saved = PlayerPrefs.GetInt(LevelStatusKeyPrefix + index, index == 0 ? (int)LevelStatus.Active : (int)LevelStatus.Locked);
            return (LevelStatus)saved;
        }

        /// <summary>Marks level done, unlocks next, advances <see cref="CurrentLevelIndex"/>, writes prefs.</summary>
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

        /// <summary>Persists current level pointer only (per-level status saved in <see cref="CompleteLevel"/>).</summary>
        public void SaveProgress()
        {
            PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevelIndex);
            PlayerPrefs.Save();
        }

        /// <summary>Restores <see cref="CurrentLevelIndex"/> from disk.</summary>
        public void LoadProgress()
        {
            CurrentLevelIndex = PlayerPrefs.GetInt(CurrentLevelKey, 0);
        }

        #endregion
    }
}
