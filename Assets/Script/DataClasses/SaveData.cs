using System;
namespace CardMatch
{
    [Serializable]
    public class LevelProgressData
    {
        public int currentLevel = 1;
        public int totalCells = 4;
        public int matchedCells = 0;
        public bool isCompleted = false;
    }

    [Serializable]
    public class SaveData
    {
        public LevelProgressData levelProgress = new LevelProgressData();
    }
}