using Enums;
using ScriptableObjects.Scripts;

namespace UI
{
    public interface ILevelService
    {
        int LevelCount { get; }
        int CurrentLevelIndex { get; set; }
        LevelData GetLevelData(int index);
        LevelStatus GetLevelStatus(int index);
        void CompleteLevel(int index);
        void SaveProgress();
        void LoadProgress();
    }
}
