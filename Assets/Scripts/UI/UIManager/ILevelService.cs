using Enums;
using ScriptableObjects.Scripts;

namespace UI
{
    /// <summary>Read level definitions and persist which levels are locked, active, or completed.</summary>
    public interface ILevelService
    {
        #region Properties

        int LevelCount { get; }
        int CurrentLevelIndex { get; set; }

        #endregion

        #region Methods

        LevelData GetLevelData(int index);
        LevelStatus GetLevelStatus(int index);
        void CompleteLevel(int index);
        void SaveProgress();
        void LoadProgress();

        #endregion
    }
}
