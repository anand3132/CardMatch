using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace CardMatch
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        private Cell lastSelected;
        private int counter = 0;
        
        [Header("Game Settings")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] public Transform cellParent;
        [SerializeField] public int pointsPerMatch = 5;
        
        public List<Cell> panels;

        private int movesCount = 0;
        private int playCount = 0;
        private int currentScore = 0;
        private int remainingTurns = 0;
        private bool isGameActive = true;

        public int seed = 0;
        private System.Random random;

        private SaveData saveData;
        private LevelManager levelManager;
        
        // Events for UI updates
        public System.Action<int> OnTurnsChanged;
        public System.Action<int> OnScoreChanged;
        public System.Action<bool> OnGameStateChanged;
        
        void Awake()
        {
            // Initialize save data early to prevent null reference exceptions
            saveData = GameProgressManager.LoadOrInitialize();
            levelManager = new LevelManager();
            random = (seed == 0) ? new System.Random() : new System.Random(seed);
            
            // Hide game area by default - only show when game starts
            HideGameArea();
        }
        
        public void StartGame()
        {
            Debug.Log("Starting game from saved progress...");
            
            // Load saved progress 
            saveData = GameProgressManager.LoadOrInitialize();
            
            // If game was over, restart the current level
            if (saveData.levelProgress.isGameOver)
            {
                Debug.Log("Previous game was over, restarting current level...");
                RestartLevel();
                return;
            }
            
            // If level was completed, move to next level
            if (saveData.levelProgress.isCompleted)
            {
                Debug.Log("Previous level was completed, moving to next level...");
                NextLevel();
                return;
            }
            
            // Otherwise, continue from where we left off
            InitializeGame();
            
            // Trigger UI events
            OnTurnsChanged?.Invoke(remainingTurns);
            OnScoreChanged?.Invoke(currentScore);
            
            Debug.Log($"Game resumed! Level {CurrentLevel}, Turns: {remainingTurns}, Score: {currentScore}");
        }
        
        public void StartNewGame()
        {
            Debug.Log("Starting completely new game...");
            ResetGame();
        }
        
        public void ResetGame()
        {
            Debug.Log("Resetting game to Level 1...");
            
            // Reset save data to level 1
            saveData.levelProgress.currentLevel = 1;
            saveData.levelProgress.totalCells = 4;
            saveData.levelProgress.matchedCells = 0;
            saveData.levelProgress.isCompleted = false;
            saveData.levelProgress.isGameOver = false;
            saveData.levelProgress.currentScore = 0;
            saveData.levelProgress.totalScore = 0;
            saveData.levelProgress.bestScore = 0;
            saveData.levelProgress.remainingTurns = 4;
            
            // Clear saved cell states for fresh start
            saveData.levelProgress.cellStates.Clear();
            
            // Save the reset data
            GameProgressManager.Save(saveData);
            
            // Initialize the first level
            InitializeGame();
            
            // Trigger UI events
            OnTurnsChanged?.Invoke(remainingTurns);
            OnScoreChanged?.Invoke(currentScore);
            
            Debug.Log($"Game reset! Level {CurrentLevel}, Turns: {remainingTurns}");
        }
        
        public void RestartLevel()
        {
            Debug.Log("Restarting current level...");
            
            // Reset current level data
            saveData.levelProgress.matchedCells = 0;
            saveData.levelProgress.isCompleted = false;
            saveData.levelProgress.isGameOver = false;
            saveData.levelProgress.currentScore = 0;
            saveData.levelProgress.remainingTurns = saveData.levelProgress.totalCells;
            
            // Clear saved cell states for fresh start
            saveData.levelProgress.cellStates.Clear();
            
            // Reset game state
            playCount = 0;
            counter = 0;
            movesCount = 0;
            lastSelected = null;
            isGameActive = true;
            currentScore = 0;
            remainingTurns = saveData.levelProgress.totalCells;
            
            // Save the reset data
            GameProgressManager.Save(saveData);
            
            // Reset all cells
            foreach (var cell in panels)
            {
                cell.Reset();
            }
            
            // Regenerate panels with new symbols
            PopulatePanels();
            
            // Trigger UI events
            OnTurnsChanged?.Invoke(remainingTurns);
            OnScoreChanged?.Invoke(currentScore);
            
            Debug.Log($"Level {CurrentLevel} restarted! Turns: {remainingTurns}");
        }
        
        public void NextLevel()
        {
            Debug.Log("Starting next level...");
            
            // Increment level
            saveData.levelProgress.currentLevel++;
            // Add 2 more cells per level
            saveData.levelProgress.totalCells += 2;
            
            // Reset level-specific data
            saveData.levelProgress.matchedCells = 0;
            saveData.levelProgress.isCompleted = false;
            saveData.levelProgress.isGameOver = false;
            saveData.levelProgress.currentScore = 0;
            
            // Clear saved cell states for new level
            saveData.levelProgress.cellStates.Clear();
            
            // Save progress
            GameProgressManager.Save(saveData);
            
            // Initialize new level
            InitializeGame();
            
            // Trigger UI events
            OnTurnsChanged?.Invoke(remainingTurns);
            OnScoreChanged?.Invoke(currentScore);
            
            Debug.Log($"Next level started! Level {CurrentLevel}, Cells: {saveData.levelProgress.totalCells}, Turns: {remainingTurns}");
        }
        
        private void InitializeGame()
        {
            // Load saved state
            remainingTurns = saveData.levelProgress.remainingTurns;
            currentScore = saveData.levelProgress.currentScore;
            isGameActive = !saveData.levelProgress.isGameOver;
            
            // Reset game state for new session
            playCount = saveData.levelProgress.matchedCells;
            movesCount = 0;
            counter = 0;
            lastSelected = null;
            
            // Generate panels
            panels = CellGenerator.GenerateCells(cellPrefab, cellParent, saveData.levelProgress.totalCells);
            PopulatePanels();
            
            // Restore cell states from saved data
            RestoreCellStates();
            
            // Show the game area
            ShowGameArea();
        }
        
        private void RestoreCellStates()
        {
            if (saveData.levelProgress.cellStates == null || saveData.levelProgress.cellStates.Count == 0)
            {
                // No saved states, all cells start face down
                foreach (var cell in panels)
                {
                    cell.SetState(false, false);
                }
                return;
            }
            
            // Restore each cell to its saved state
            for (int i = 0; i < panels.Count && i < saveData.levelProgress.cellStates.Count; i++)
            {
                var cellState = saveData.levelProgress.cellStates[i];
                var cell = panels[i];
                
                if (cell != null)
                {
                    cell.SetState(cellState.isFlipped, cellState.isMatched);
                }
            }
        }
        
        private void SaveCellStates()
        {
            if (panels == null) return;
            
            saveData.levelProgress.cellStates.Clear();
            
            for (int i = 0; i < panels.Count; i++)
            {
                var cell = panels[i];
                if (cell != null)
                {
                    saveData.levelProgress.cellStates.Add(cell.GetState(i));
                }
            }
        }
        
        private void ShowGameArea()
        {
            if (cellParent != null)
            {
                cellParent.gameObject.SetActive(true);
            }
        }
        
        private void HideGameArea()
        {
            if (cellParent != null)
            {
                cellParent.gameObject.SetActive(false);
            }
        }

        public void CurrentMove(Cell cell)
        {
            if (!isGameActive) return;
            
            movesCount++;

            if (counter == 0)
            {
                lastSelected = cell;
                counter++;
                return;
            }

            counter = 0;

            if (lastSelected.cellID != cell.cellID)
            {
                // Wrong match - reduce turns
                lastSelected.Reset();
                cell.Reset();
                ReduceTurns();
            }
            else
            {
                // Correct match - increase score
                playCount += 2;
                currentScore += pointsPerMatch;
                saveData.levelProgress.matchedCells = playCount;
                saveData.levelProgress.currentScore = currentScore;
                
                // Mark both cells as matched
                lastSelected.SetState(true, true);
                cell.SetState(true, true);
                
                // Save cell states
                SaveCellStates();
                
                OnScoreChanged?.Invoke(currentScore);

                if (playCount >= panels.Count)
                {
                    // Level completed successfully
                    LevelCompleted();
                    return;
                }

                GameProgressManager.Save(saveData);
            }

            lastSelected = null;
        }

        private void ReduceTurns()
        {
            remainingTurns--;
            saveData.levelProgress.remainingTurns = remainingTurns;
            
            OnTurnsChanged?.Invoke(remainingTurns);
            
            if (remainingTurns <= 0)
            {
                GameOver();
            }
            
            GameProgressManager.Save(saveData);
        }

        private void LevelCompleted()
        {
            isGameActive = false;
            saveData.levelProgress.isCompleted = true;
            saveData.levelProgress.currentLevel++;
            saveData.levelProgress.totalCells += 2;
            saveData.levelProgress.matchedCells = 0;
            
            // Update best score
            if (currentScore > saveData.levelProgress.bestScore)
            {
                saveData.levelProgress.bestScore = currentScore;
            }
            
            // Add to total score
            saveData.levelProgress.totalScore += currentScore;
            
            GameProgressManager.Save(saveData);
            
            // Trigger UI events
            OnGameStateChanged?.Invoke(true); // true = win
            
            Debug.Log($"Level Completed! Score: {currentScore}, Best: {saveData.levelProgress.bestScore}");
        }

        private void GameOver()
        {
            isGameActive = false;
            saveData.levelProgress.isGameOver = true;
            
            GameProgressManager.Save(saveData);
            
            // Trigger UI events
            OnGameStateChanged?.Invoke(false); // false = lose
            
            Debug.Log("Game Over! Out of turns!");
        }

        // public void Reset()
        // {
        //     RestartLevel(); // Use the new UX function
        // }

        // public void StartNewLevel()
        // {
        //     NextLevel(); // Use the new UX function
        // }



        void PopulatePanels()
        {
            int requiredCells = saveData.levelProgress.totalCells;

            if (requiredCells % 2 != 0)
                requiredCells++;

            if (panels.Count < requiredCells)
            {
                Debug.LogWarning($"Not enough panels in scene. Needed: {requiredCells}, Found: {panels.Count}");
                return;
            }

            panels = panels.Take(requiredCells).ToList();

            if (panels.Count % 2 != 0)
            {
                Debug.LogError("Odd number of panels detected!");
                return;
            }

            int pairCount = panels.Count / 2;
            var symbols = levelManager.GenerateSymbolPairs(pairCount);
            levelManager.Shuffle(symbols, random);

            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Initialise(symbols[i]);
            }
        }

        // Public getters for UI access
        public int CurrentMoves => movesCount;
        public int RemainingTurns => remainingTurns;
        public int CurrentScore => currentScore;
        public int TotalScore => saveData?.levelProgress.totalScore ?? 0;
        public int BestScore => saveData?.levelProgress.bestScore ?? 0;
        public int CurrentLevel => saveData?.levelProgress.currentLevel ?? 1;
        public bool IsGameActive => isGameActive;
        public bool IsGameOver => saveData?.levelProgress.isGameOver ?? false;
        public bool IsLevelCompleted => saveData?.levelProgress.isCompleted ?? false;
        
        // .. !! Check if the manager is properly initialized
        public bool IsInitialized => saveData != null;
        
        // Debug method to show cell states
        [ContextMenu("Debug Cell States")]
        public void DebugCellStates()
        {
            if (saveData?.levelProgress.cellStates != null)
            {
                Debug.Log($"Saved Cell States: {saveData.levelProgress.cellStates.Count}");
                for (int i = 0; i < saveData.levelProgress.cellStates.Count; i++)
                {
                    var state = saveData.levelProgress.cellStates[i];
                    Debug.Log($"Cell {i}: ID={state.cellID}, Flipped={state.isFlipped}, Matched={state.isMatched}");
                }
            }
            else
            {
                Debug.Log("No saved cell states");
            }
        }
        
    }
}
