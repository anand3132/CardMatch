using UnityEngine;

namespace CardMatch
{
    public class UXManager : Singleton<UXManager>
    {
        [SerializeField] private AllInOneScreen allInOneScreen;
        [SerializeField] private HUDScreen hudScreen;

        private IUIScreen[] allScreens;
        private IUIScreen currentScreen;

        protected override void Awake()
        {
            base.Awake();
            allScreens = new IUIScreen[] { allInOneScreen, hudScreen };
            foreach (var screen in allScreens)
            {
                screen.Initialize();
                screen.Hide();
            }
        }
        
        void Start()
        {
            // Wait for GamePlayManager to be properly initialized
            var gameManager = GamePlayManager.Instance;
            
            // Subscribe to game events
            if (gameManager != null)
            {
                gameManager.OnTurnsChanged += OnTurnsChanged;
                gameManager.OnScoreChanged += OnScoreChanged;
                gameManager.OnGameStateChanged += OnGameStateChanged;
            }
            
            // Check if there is saved progress
            if (gameManager.IsInitialized && (gameManager.CurrentLevel > 1 || gameManager.TotalScore > 0))
            {
                // Show continue game option
                ShowGameStart(() => StartGameAndShowHUD(), true);
            }
            else
            {
                // Show new game option
                ShowGameStart(() => StartGameAndShowHUD(), false);
            }
        }
        
        private void OnDestroy()
        {
            var gameManager = GamePlayManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnTurnsChanged -= OnTurnsChanged;
                gameManager.OnScoreChanged -= OnScoreChanged;
                gameManager.OnGameStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnTurnsChanged(int turns)
        {
            if (currentScreen == hudScreen)
            {
                hudScreen.RefreshFromGameManager();
            }
        }
        
        private void OnScoreChanged(int score)
        {
            if (currentScreen == hudScreen)
            {
                hudScreen.RefreshFromGameManager();
            }
        }
        
        private void OnGameStateChanged(bool isWin)
        {
            if (isWin)
            {
                ShowGameWon(() => NextLevelAndShowHUD());
            }
            else
            {
                ShowGameLost(() => RestartLevelAndShowHUD());
            }
        }
        
        private void StartGameAndShowHUD()
        {
            var gameManager = GamePlayManager.Instance;
            
            // Check if there is saved progress
            if (gameManager.IsInitialized && (gameManager.CurrentLevel > 1 || gameManager.TotalScore > 0))
            {
                // Continue from saved progress
                gameManager.StartGame();
            }
            else
            {
                // Start completely new game
                gameManager.StartNewGame();
            }
            
            ShowHUD();
        }

        public void ShowGameStart(System.Action onStart, bool hasProgress = false)
        {
            // Hide game area when showing start screen
            HideGameArea();
            
            SwitchTo(allInOneScreen);
            allInOneScreen.Show(AllInOneScreenMode.GameStart, onStart, hasProgress);
        }
        
        public void ShowNewGameOptions()
        {
            var gameManager = GamePlayManager.Instance;
            if (gameManager.IsInitialized && (gameManager.CurrentLevel > 1 || gameManager.TotalScore > 0))
            {
                // Show options for continuing or starting fresh
                ShowGameStart(() => StartGameAndShowHUD(), true);
            }
            else
            {
                // Show new game option
                ShowGameStart(() => StartGameAndShowHUD(), false);
            }
        }
        
        public void ResetGame()
        {
            var gameManager = GamePlayManager.Instance;
            gameManager.ResetGame();
            ShowGameStart(() => StartGameAndShowHUD(), false);
        }
        
        public void HideGameArea()
        {
            var gameManager = GamePlayManager.Instance;
            if (gameManager != null && gameManager.cellParent != null)
            {
                gameManager.cellParent.gameObject.SetActive(false);
            }
        }

        public void ShowGameWon(System.Action onNextLevel)
        {
            SwitchTo(allInOneScreen);
            allInOneScreen.Show(AllInOneScreenMode.GameWon, onNextLevel);
        }

        public void ShowGameLost(System.Action onRetry)
        {
            SwitchTo(allInOneScreen);
            allInOneScreen.Show(AllInOneScreenMode.GameLost, onRetry);
        }
        
        public void NextLevelAndShowHUD()
        {
            var gameManager = GamePlayManager.Instance;
            gameManager.NextLevel();
            ShowHUD();
        }
        
        public void RestartLevelAndShowHUD()
        {
            var gameManager = GamePlayManager.Instance;
            gameManager.RestartLevel();
            ShowHUD();
        }

        public void ShowHUD()
        {
            SwitchTo(hudScreen);
        }
        
        // Debug methods for testing
        [ContextMenu("Show Start Screen")]
        public void DebugShowStartScreen()
        {
            ShowGameStart(() => StartGameAndShowHUD(), false);
        }
        
        [ContextMenu("Show Continue Screen")]
        public void DebugShowContinueScreen()
        {
            ShowGameStart(() => StartGameAndShowHUD(), true);
        }
        
        [ContextMenu("Hide Game Area")]
        public void DebugHideGameArea()
        {
            HideGameArea();
        }
        
        [ContextMenu("Show Game State")]
        public void DebugShowGameState()
        {
            var gameManager = GamePlayManager.Instance;
            if (gameManager != null)
            {
                Debug.Log($"Game State: Level={gameManager.CurrentLevel}, Score={gameManager.TotalScore}, Initialized={gameManager.IsInitialized}");
            }
        }

        private void SwitchTo(IUIScreen newScreen)
        {
            currentScreen?.Hide();
            currentScreen = newScreen;
            currentScreen.Show();
            currentScreen.UpdateUI();
        }
    }


}