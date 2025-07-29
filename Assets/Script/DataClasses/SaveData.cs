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
        
        public int totalTurns = 4;        // Total turns available (equal to cell count)
        public int remainingTurns = 4;    // Current remaining turns
        public int currentScore = 0;      // Current level score
        public int totalScore = 0;        // Total game score
        public int bestScore = 0;         // Best score for this level
        public bool isGameOver = false;   // Game over when turns run out
    }

    [Serializable]
    public class SaveData
    {
        public LevelProgressData levelProgress = new LevelProgressData();
    }
}