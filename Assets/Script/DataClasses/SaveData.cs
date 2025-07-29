using System;
using System.Collections.Generic;

namespace CardMatch
{
    [Serializable]
    public class CellState
    {
        public string cellID;           // SymbolID of the cell
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
        
        public int totalTurns = 4;        // Total turns available
        public int remainingTurns = 4;    // Current remaining turns
        public int currentScore = 0;      // Current level score
        public bool isGameOver = false;   // Game over when turns run out
        
        public List<CellState> cellStates = new List<CellState>();
        public List<string> symbolArrangement = new List<string>();  // Save the symbol arrangement
        
        // Reset level-specific data (keeps level and total cells)
        public void ResetLevelData()
        {
            matchedCells = 0;
            isCompleted = false;
            isGameOver = false;
            currentScore = 0;
            remainingTurns = totalCells;
            cellStates.Clear();
            symbolArrangement.Clear();
        }
        
        // Move to next level
        public void NextLevel()
        {
            currentLevel++;
            totalCells += 2;
            ResetLevelData();
        }
    }

    [Serializable]
    public class SaveData
    {
        public LevelProgressData levelProgress = new LevelProgressData();
        public int totalScore = 0; 
    }
}