using System;
using System.Collections.Generic;

namespace CardMatch
{
    [Serializable]
    public class CellState
    {
        public string cellID;           // Symbol/ID of the cell
        public bool isMatched;          // Whether this cell is part of a matched pair
        public bool isFlipped;          // Whether this cell is currently flipped open
        public int cellIndex;           // Position index of the cell
    }

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
        
        // New: Store cell states for matched pairs
        public List<CellState> cellStates = new List<CellState>();
    }

    [Serializable]
    public class SaveData
    {
        public LevelProgressData levelProgress = new LevelProgressData();
    }
}