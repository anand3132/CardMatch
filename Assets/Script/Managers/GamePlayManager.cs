using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CardMatch
{
    public class GamePlayManager : Singleton<GamePlayManager>, IManager
    {
        #region Fields and Properties
        

        
        [Header("Game Settings")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private GameObject cellHolderPrefab;
        [SerializeField] private Transform gameAreaParent;
        [SerializeField] public int pointsPerMatch = 5;
        [SerializeField] public int bonusPointsPerRemainingTurn = 10;
        
        private List<Cell> panels;
        private Transform cellParent;
        private int movesCount = 0;
        private int playCount = 0;
        private int currentScore = 0;
        private int remainingTurns = 0;
        private bool isGameActive = true;

        public int seed = 0;
        private System.Random random;
        private LevelManager levelManager;
        
        // Events for UI updates
        public System.Action<int> OnTurnsChanged;
        public System.Action<int> OnScoreChanged;
        public System.Action<bool> OnGameStateChanged;
        
        #endregion

        #region IManager Implementation
        
        public bool IsInitialized => SaveSystem.GameData != null && levelManager != null && cellPrefab != null && cellHolderPrefab != null && gameAreaParent != null;
        
        public void Initialize()
        {
            levelManager = new LevelManager();
            random = (seed == 0) ? new System.Random() : new System.Random(seed);
        }
        
        public void Cleanup()
        {
            levelManager = null;
            random = null;
        }
        
        #endregion

        #region Game Flow Control
        
        public void StartGame()
        {
            if (SaveSystem.GameData.levelProgress.isGameOver)
            {
                RestartLevel();
                return;
            }
            
            if (SaveSystem.GameData.levelProgress.isCompleted)
            {
                NextLevel();
                return;
            }
            
            InitializeGame();
            TriggerUIEvents();
        }
        
        public void StartNewGame() => ResetGame();
        
        public void ResetGame()
        {
            SaveSystem.GameData.levelProgress = new LevelProgressData();
            SaveSystem.Save();
            InitializeGame();
            TriggerUIEvents();
        }
        
        public void RestartLevel()
        {
            SaveSystem.GameData.levelProgress.ResetLevelData();
            ResetGameState();
            SaveSystem.Save();
            CellSystem.ResetAllCells();
            InitializeGame();
            TriggerUIEvents();
        }
        

        
        public void NextLevel()
        {
            SaveSystem.GameData.levelProgress.NextLevel();
            SaveSystem.Save();
            InitializeGame();
            TriggerUIEvents();
        }
        
        #endregion

        #region Game Initialization
        
        private void InitializeGame()
        {
            LoadSavedState();
            ResetGameSession();
            GenerateAndSetupCells();
            TriggerUIEvents();
        }
        
        private void GenerateAndSetupCells()
        {
            if (cellPrefab == null || cellHolderPrefab == null || gameAreaParent == null)
            {
                Debug.LogError("GamePlayManager: Cell prefab, cell holder prefab, or game area parent not assigned!");
                return;
            }
            
            // Clear any existing cell parent
            if (cellParent != null)
            {
                DestroyImmediate(cellParent.gameObject);
            }
            
            // Instantiate the CellHolder prefab
            GameObject cellHolderInstance = Instantiate(cellHolderPrefab, gameAreaParent);
            cellParent = cellHolderInstance.transform;
            
            // Generate cells using CellSystem
            panels = CellSystem.GenerateCells(cellPrefab, cellParent, SaveSystem.GameData.levelProgress.totalCells);
            
            // Populate cells with symbols
            CellSystem.PopulateCells(panels, levelManager, random, SaveSystem.GameData.levelProgress.totalCells);
            
            // Restore cell states if available
            CellSystem.RestoreCellStates(panels);
            
            // Show game area
            cellParent?.gameObject.SetActive(true);
        }
        
        private void LoadSavedState()
        {
            remainingTurns = SaveSystem.GameData.levelProgress.remainingTurns;
            currentScore = SaveSystem.GameData.levelProgress.currentScore;
            isGameActive = !SaveSystem.GameData.levelProgress.isGameOver;
        }
        
        private void ResetGameSession()
        {
            playCount = SaveSystem.GameData.levelProgress.matchedCells;
            movesCount = 0;
            CellSystem.ResetCellSelection();
        }
        
        private void ResetGameState()
        {
            playCount = 0;
            movesCount = 0;
            isGameActive = true;
            currentScore = 0;
            remainingTurns = SaveSystem.GameData.levelProgress.totalCells;
            CellSystem.ResetCellSelection();
        }
        
        #endregion

        #region Game Logic
        
        public void CurrentMove(Cell cell)
        {
            if (!isGameActive) return;
            
            movesCount++;
            bool isCorrectMatch = CellSystem.ProcessCellMove(cell);
            
            if (isCorrectMatch)
            {
                HandleCorrectMatch();
            }
            else if (CellSystem.IsWrongMatch())
            {
                HandleWrongMatch();
            }
        }
        
        private void HandleWrongMatch()
        {
            // Play mismatch sound
            SoundManager.Instance?.PlayCardMismatch();
            ReduceTurns();
        }
        
        private void HandleCorrectMatch()
        {
            // Play match sound
            SoundManager.Instance?.PlayCardMatch();
            
            playCount += 2;
            currentScore += pointsPerMatch;
            SaveSystem.GameData.levelProgress.matchedCells = playCount;
            SaveSystem.GameData.levelProgress.currentScore = currentScore;
            
            OnScoreChanged?.Invoke(currentScore);

            if (playCount >= GetTotalCells())
            {
                LevelCompleted();
                return;
            }

            SaveSystem.Save();
        }
        
        #endregion

        #region Game State Management
        
        private void ReduceTurns()
        {
            remainingTurns--;
            SaveSystem.GameData.levelProgress.remainingTurns = remainingTurns;
            
            OnTurnsChanged?.Invoke(remainingTurns);
            
            if (remainingTurns <= 0)
            {
                GameOver();
            }
            
            SaveSystem.Save();
        }

        private void LevelCompleted()
        {
            isGameActive = false;
            SaveSystem.GameData.levelProgress.isCompleted = true;
            
            int bonusPoints = remainingTurns * bonusPointsPerRemainingTurn;
            currentScore += bonusPoints;
            
            // Add current level score to total score before resetting
            SaveSystem.GameData.totalScore += currentScore;
            
            // Level increment moved to NextLevel() method to avoid double increment
            SaveSystem.GameData.levelProgress.totalCells += 2;
            SaveSystem.GameData.levelProgress.matchedCells = 0;
            SaveSystem.GameData.levelProgress.currentScore = 0;
            
            SaveSystem.Save();
            OnGameStateChanged?.Invoke(true);
            OnScoreChanged?.Invoke(currentScore);
            
            // Play win sound and show win screen
            SoundManager.Instance?.PlayGameWin();
            UXManager.Instance?.ShowGameWon();
        }

        private void GameOver()
        {
            // Play game over sound
            SoundManager.Instance?.PlayGameOver();
            
            isGameActive = false;
            SaveSystem.GameData.levelProgress.isGameOver = true;
            SaveSystem.Save();
            OnGameStateChanged?.Invoke(false);
            
            // Show lose screen directly
            UXManager.Instance?.ShowGameLost();
        }
        
        #endregion

        #region UI Events
        
        private void TriggerUIEvents()
        {
            OnTurnsChanged?.Invoke(remainingTurns);
            OnScoreChanged?.Invoke(currentScore); 
        }
        
        #endregion

        #region Public Getters
        
        public int CurrentMoves => movesCount;
        public int RemainingTurns => remainingTurns;
        public int CurrentScore => SaveSystem.GameData?.totalScore ?? 0;
        // Score for current level only
        public int CurrentLevelScore => currentScore; 
        public int CurrentLevel => SaveSystem.GameData?.levelProgress.currentLevel ?? 1;
        public bool IsGameActive => isGameActive;
        public bool IsGameOver => SaveSystem.GameData?.levelProgress.isGameOver ?? false;
        public bool IsLevelCompleted => SaveSystem.GameData?.levelProgress.isCompleted ?? false;
        public int GetTotalCells() => CellSystem.GetTotalCells();
        public List<Cell> GetCurrentPanels() => CellSystem.GetCurrentPanels();
        
        #endregion
    }
}
