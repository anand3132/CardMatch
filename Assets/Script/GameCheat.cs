using UnityEngine;

namespace CardMatch
{
    // Debug cheat system for testing game scenarios
    public class GameCheat : MonoBehaviour
    {
        [Header("Cheat Settings")]
        [SerializeField] private bool enableCheats = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;
        
        [Header("Auto Play Settings")]
        [SerializeField] private bool enableAutoPlay = false;
        [SerializeField] private float autoPlayDelay = 1.5f; // Increased delay between moves
        [SerializeField] private float cellFlipDelay = 0.8f; // Wait for cell flip animation
        
        private bool showCheatGUI = false;
        private Rect windowRect = new Rect(10, 10, 400, 600); // Increased from 300x400 to 400x600
        private bool isAutoPlaying = false;
        private System.Collections.IEnumerator autoPlayCoroutine;
        
        // Foldable section states
        private bool gameStateExpanded = true;
        private bool gameFlowExpanded = true;
        private bool levelManagementExpanded = false;
        private bool scoreManagementExpanded = false;
        private bool gameResetExpanded = false;
        private bool autoPlayExpanded = true;
        
        void Update()
        {
            // Toggle cheat GUI with F1 key (editor only)
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleCheatGUI();
            }
        }
        
        // Public method to toggle cheat GUI from button click (mobile support)
        public void ToggleCheatGUI()
        {
            showCheatGUI = !showCheatGUI;
            Debug.Log($"GameCheat: GUI toggled - {(showCheatGUI ? "Shown" : "Hidden")}");
        }
        
        // Public method to show cheat GUI
        public void ShowCheatGUI()
        {
            showCheatGUI = true;
            Debug.Log("GameCheat: GUI shown");
        }
        
        // Public method to hide cheat GUI
        public void HideCheatGUI()
        {
            showCheatGUI = false;
            Debug.Log("GameCheat: GUI hidden");
        }
        
        void OnGUI()
        {
            if (!enableCheats || !showCheatGUI) return;
            
            // Create cheat window
            windowRect = GUI.Window(0, windowRect, DrawCheatWindow, "Game Cheats");
        }
        
        void DrawCheatWindow(int windowID)
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            
            // Game State Info
            DrawFoldableSection("=== GAME STATE ===", ref gameStateExpanded, () => {
                if (SaveSystem.GameData != null)
                {
                    GUILayout.Label($"Level: {SaveSystem.GameData.levelProgress.currentLevel}", GUILayout.Height(25));
                    GUILayout.Label($"Cells: {SaveSystem.GameData.levelProgress.totalCells}", GUILayout.Height(25));
                    GUILayout.Label($"Score: {SaveSystem.GameData.totalScore}", GUILayout.Height(25));
                    GUILayout.Label($"Turns: {SaveSystem.GameData.levelProgress.remainingTurns}", GUILayout.Height(25));
                    GUILayout.Label($"Matched: {SaveSystem.GameData.levelProgress.matchedCells}", GUILayout.Height(25));
                }
                else
                {
                    GUILayout.Label("No save data", GUILayout.Height(25));
                }
            });
            
            GUILayout.Space(10);
            
            // Game Flow Controls
            DrawFoldableSection("=== GAME FLOW ===", ref gameFlowExpanded, () => {
                if (GUILayout.Button("Win Current Level", GUILayout.Height(35)))
                {
                    CheatWinLevel();
                }
                
                if (GUILayout.Button("Lose Current Level", GUILayout.Height(35)))
                {
                    CheatLoseLevel();
                }
                
                if (GUILayout.Button("Next Level", GUILayout.Height(35)))
                {
                    CheatNextLevel();
                }
                
                if (GUILayout.Button("Restart Level", GUILayout.Height(35)))
                {
                    CheatRestartLevel();
                }
            });
            
            GUILayout.Space(10);
            
            // Level Management
            DrawFoldableSection("=== LEVEL MANAGEMENT ===", ref levelManagementExpanded, () => {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Level 1", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(1);
                }
                if (GUILayout.Button("Level 5", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(5);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Level 10", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(10);
                }
                if (GUILayout.Button("Level 15", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(15);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Level 20", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(20);
                }
                if (GUILayout.Button("Level 24", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(24);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Level 25", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(25);
                }
                if (GUILayout.Button("Level 30", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(30);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Level 35", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(35);
                }
                if (GUILayout.Button("Level 40", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatGoToLevel(40);
                }
                GUILayout.EndHorizontal();
            });
            
            GUILayout.Space(10);
            
            // Score Management
            DrawFoldableSection("=== SCORE MANAGEMENT ===", ref scoreManagementExpanded, () => {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+100 Points", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatAddScore(100);
                }
                if (GUILayout.Button("+500 Points", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    CheatAddScore(500);
                }
                GUILayout.EndHorizontal();
                
                if (GUILayout.Button("Reset Score", GUILayout.Height(35)))
                {
                    CheatResetScore();
                }
            });
            
            GUILayout.Space(10);
            
            // Game Reset
            DrawFoldableSection("=== GAME RESET ===", ref gameResetExpanded, () => {
                if (GUILayout.Button("Reset Game", GUILayout.Height(35)))
                {
                    CheatResetGame();
                }
                
                if (GUILayout.Button("Delete Save Data", GUILayout.Height(35)))
                {
                    CheatDeleteSaveData();
                }
            });
            
            GUILayout.Space(10);
            
            // Auto Play
            DrawFoldableSection("=== AUTO PLAY ===", ref autoPlayExpanded, () => {
                // Speed controls
                GUILayout.Label("Speed Settings:", GUILayout.Height(20));
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Move Delay:", GUILayout.Width(80));
                autoPlayDelay = GUILayout.HorizontalSlider(autoPlayDelay, 0.5f, 3f, GUILayout.ExpandWidth(true));
                GUILayout.Label($"{autoPlayDelay:F1}s", GUILayout.Width(30));
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Flip Delay:", GUILayout.Width(80));
                cellFlipDelay = GUILayout.HorizontalSlider(cellFlipDelay, 0.3f, 2f, GUILayout.ExpandWidth(true));
                GUILayout.Label($"{cellFlipDelay:F1}s", GUILayout.Width(30));
                GUILayout.EndHorizontal();
                
                GUILayout.Space(5);
                
                if (!isAutoPlaying)
                {
                    if (GUILayout.Button("Start Auto Play", GUILayout.Height(35)))
                    {
                        StartAutoPlay();
                    }
                }
                else
                {
                    if (GUILayout.Button("Stop Auto Play", GUILayout.Height(35)))
                    {
                        StopAutoPlay();
                    }
                }
                
                if (GUILayout.Button("Auto Play Current Level", GUILayout.Height(35)))
                {
                    AutoPlayCurrentLevel();
                }
            });
            
            GUILayout.Space(10);
            
            // Close button
            if (GUILayout.Button("Close", GUILayout.Height(40)))
            {
                showCheatGUI = false;
            }
            
            GUILayout.EndVertical();
            
            // Make window draggable
            GUI.DragWindow();
        }
        
        // Helper method to draw foldable sections
        private void DrawFoldableSection(string title, ref bool isExpanded, System.Action content)
        {
            // Create clickable header with expand/collapse indicator
            string headerText = isExpanded ? $"▼ {title}" : $"▶ {title}";
            
            if (GUILayout.Button(headerText, GUI.skin.box, GUILayout.ExpandWidth(true)))
            {
                isExpanded = !isExpanded;
            }
            
            // Draw content if expanded
            if (isExpanded)
            {
                GUILayout.BeginVertical("box");
                content?.Invoke();
                GUILayout.EndVertical();
            }
        }
        
        // Cheat Methods
        private void CheatWinLevel()
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log("Cheat: Winning current level");
            
            // Set all cells as matched
            SaveSystem.GameData.levelProgress.matchedCells = SaveSystem.GameData.levelProgress.totalCells;
            SaveSystem.GameData.levelProgress.isCompleted = true;
            SaveSystem.GameData.levelProgress.isGameOver = false;
            
            // Add some score
            SaveSystem.GameData.levelProgress.currentScore = 100;
            
            SaveSystem.Save();
            
            // Trigger win screen
            if (UXManager.Instance != null)
            {
                UXManager.Instance.ShowGameWon();
            }
        }
        
        private void CheatLoseLevel()
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log("Cheat: Losing current level");
            
            // Set game over
            SaveSystem.GameData.levelProgress.isGameOver = true;
            SaveSystem.GameData.levelProgress.isCompleted = false;
            SaveSystem.GameData.levelProgress.remainingTurns = 0;
            
            SaveSystem.Save();
            
            // Trigger lose screen
            if (UXManager.Instance != null)
            {
                UXManager.Instance.ShowGameLost();
            }
        }
        
        private void CheatNextLevel()
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log("Cheat: Going to next level");
            
            SaveSystem.GameData.levelProgress.NextLevel();
            SaveSystem.Save();
            
            // Restart game to apply new level
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.StartGame();
            }
        }
        
        private void CheatRestartLevel()
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log("Cheat: Restarting current level");
            
            SaveSystem.GameData.levelProgress.ResetLevelData();
            SaveSystem.Save();
            
            // Restart game
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.StartGame();
            }
        }
        
        private void CheatGoToLevel(int level)
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log($"Cheat: Going to level {level}");
            
            // Calculate cells for target level
            int targetCells = 4 + (level - 1) * 2;
            
            SaveSystem.GameData.levelProgress.currentLevel = level;
            SaveSystem.GameData.levelProgress.totalCells = targetCells;
            SaveSystem.GameData.levelProgress.ResetLevelData();
            
            SaveSystem.Save();
            
            // Restart game to apply new level
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.StartGame();
            }
        }
        
        private void CheatAddScore(int points)
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log($"Cheat: Adding {points} points");
            
            SaveSystem.GameData.totalScore += points;
            SaveSystem.Save();
            
            // Update UI if needed
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.OnScoreChanged?.Invoke(SaveSystem.GameData.totalScore);
            }
        }
        
        private void CheatResetScore()
        {
            if (SaveSystem.GameData == null) return;
            
            Debug.Log("Cheat: Resetting score");
            
            SaveSystem.GameData.totalScore = 0;
            SaveSystem.Save();
            
            // Update UI if needed
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.OnScoreChanged?.Invoke(0);
            }
        }
        
        private void CheatResetGame()
        {
            Debug.Log("Cheat: Resetting entire game");
            
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.ResetGame();
            }
        }
        
        private void CheatDeleteSaveData()
        {
            Debug.Log("Cheat: Deleting save data");
            
            SaveSystem.Delete();
            
            // Restart game
            if (GamePlayManager.Instance != null)
            {
                GamePlayManager.Instance.StartGame();
            }
        }
        
        // Auto Play Methods
        private void StartAutoPlay()
        {
            if (isAutoPlaying) return;
            
            Debug.Log("Cheat: Starting auto play");
            isAutoPlaying = true;
            autoPlayCoroutine = AutoPlayCoroutine();
            StartCoroutine(autoPlayCoroutine);
        }
        
        private void StopAutoPlay()
        {
            if (!isAutoPlaying) return;
            
            Debug.Log("Cheat: Stopping auto play");
            isAutoPlaying = false;
            
            if (autoPlayCoroutine != null)
            {
                StopCoroutine(autoPlayCoroutine);
                autoPlayCoroutine = null;
            }
        }
        
        private void AutoPlayCurrentLevel()
        {
            if (isAutoPlaying) return;
            
            Debug.Log("Cheat: Auto playing current level");
            StartCoroutine(AutoPlayCurrentLevelCoroutine());
        }
        
        private System.Collections.IEnumerator AutoPlayCoroutine()
        {
            Debug.Log("Auto Play: Starting continuous auto play");
            
            while (isAutoPlaying)
            {
                // Check if game is active and ready
                if (GamePlayManager.Instance == null)
                {
                    Debug.LogWarning("Auto Play: GamePlayManager not found");
                    yield return new WaitForSeconds(2f);
                    continue;
                }
                
                if (!GamePlayManager.Instance.IsGameActive)
                {
                    Debug.Log("Auto Play: Game not active, waiting...");
                    yield return new WaitForSeconds(2f);
                    continue;
                }
                
                // Auto play current level
                yield return StartCoroutine(AutoPlayCurrentLevelCoroutine());
                
                // Wait for level completion and UI transitions
                yield return new WaitForSeconds(3f);
                
                // Check if we should continue to next level
                if (SaveSystem.GameData?.levelProgress.isCompleted == true)
                {
                    Debug.Log("Auto Play: Level completed, waiting for next level...");
                    yield return new WaitForSeconds(2f);
                }
            }
            
            Debug.Log("Auto Play: Continuous auto play stopped");
        }
        
        private System.Collections.IEnumerator AutoPlayCurrentLevelCoroutine()
        {
            Debug.Log("Auto Play: Starting current level auto play");
            
            // Wait for game to be ready
            yield return new WaitForSeconds(1f);
            
            // Get current panels
            var panels = CellSystem.GetCurrentPanels();
            if (panels == null || panels.Count == 0)
            {
                Debug.LogWarning("Auto Play: No panels found");
                yield break;
            }
            
            Debug.Log($"Auto Play: Found {panels.Count} cells");
            
            // Create a map of cell IDs to cells for easy lookup
            var cellMap = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Cell>>();
            
            foreach (var cell in panels)
            {
                if (cell == null) continue;
                
                if (!cellMap.ContainsKey(cell.cellID))
                {
                    cellMap[cell.cellID] = new System.Collections.Generic.List<Cell>();
                }
                cellMap[cell.cellID].Add(cell);
            }
            
            Debug.Log($"Auto Play: Found {cellMap.Count} unique cell types");
            
            // Play through all pairs
            foreach (var pair in cellMap)
            {
                if (!isAutoPlaying) 
                {
                    Debug.Log("Auto Play: Stopped by user");
                    yield break;
                }
                
                // Check if game is still active
                if (GamePlayManager.Instance == null || !GamePlayManager.Instance.IsGameActive)
                {
                    Debug.Log("Auto Play: Game became inactive, stopping");
                    yield break;
                }
                
                var cells = pair.Value;
                if (cells.Count >= 2)
                {
                    var cell1 = cells[0];
                    var cell2 = cells[1];
                    
                    // Check if cells are already matched
                    if (cell1 == null || cell2 == null) continue;
                    
                    Debug.Log($"Auto Play: Matching cells with ID '{cell1.cellID}'");
                    
                    // Click first cell
                    Debug.Log($"Auto Play: Clicking first cell");
                    cell1.OnPanelClick();
                    yield return new WaitForSeconds(cellFlipDelay); // Wait for flip animation
                    
                    // Click second cell
                    Debug.Log($"Auto Play: Clicking second cell");
                    cell2.OnPanelClick();
                    yield return new WaitForSeconds(cellFlipDelay); // Wait for flip animation
                    
                    // Wait between pairs
                    yield return new WaitForSeconds(autoPlayDelay);
                }
            }
            
            Debug.Log("Auto Play: Level auto play completed");
            
            // Wait for any final animations
            yield return new WaitForSeconds(1f);
        }
    }
} 