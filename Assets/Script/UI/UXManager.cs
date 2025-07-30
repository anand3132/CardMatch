using UnityEngine;

namespace CardMatch
{
    // Manages UI screen transitions and navigation
    public class UXManager : Singleton<UXManager>, IManager
    {
        [SerializeField] private AllInOneScreen allInOneScreen;
        [SerializeField] private HUDScreen hudScreen;

        private IUIScreen[] allScreens;
        private IUIScreen currentScreen;

        // IManager Implementation
        public bool IsInitialized => allScreens != null && allScreens.Length > 0;
        
        // Initialize all UI screens and hide them
        public void Initialize()
        {
            allScreens = new IUIScreen[] { allInOneScreen, hudScreen };
            foreach (var screen in allScreens)
            {
                screen.Initialize();
                screen.Hide();
            }
        }
        
        // Hide all screens and clear references
        public void Cleanup()
        {
            // Hide all screens
            if (allScreens != null)
            {
                foreach (var screen in allScreens)
                {
                    screen.Hide();
                }
            }
            
            // Clear references
            allScreens = null;
            currentScreen = null;
        }

        // Shows a screen with specific context data
        public void ShowScreenWithContext(IUIScreen screen, UIContextData contextData)
        {
            SwitchTo(screen);
            screen.HandleContext(contextData);
        }
        
        // Shows the game start screen with context
        public void ShowGameStart()
        {
            var contextData = UIContextData.CreateGameStart();
            
            contextData.onContinue = () => {
                GamePlayManager.Instance.StartGame();
                ShowHUD();
            };
            
            ShowScreenWithContext(allInOneScreen, contextData);
        }
        
        // Shows the game won screen with context
        public void ShowGameWon()
        {
            var contextData = UIContextData.CreateGameWon();
            contextData.onNextLevel = () => {
                GamePlayManager.Instance.NextLevel();
                ShowHUD();
            };
            
            ShowScreenWithContext(allInOneScreen, contextData);
        }
        
        // Shows the game lost screen with context
        public void ShowGameLost()
        {
            var contextData = UIContextData.CreateGameLost();
            contextData.onRestart = () => {
                GamePlayManager.Instance.RestartLevel();
                ShowHUD();
            };
            ShowScreenWithContext(allInOneScreen, contextData);
        }
        
        // Shows the HUD screen
        public void ShowHUD()
        {
            var contextData = new UIContextData(UIContext.GameStart); // HUD doesn't need specific context
            ShowScreenWithContext(hudScreen, contextData);
        }
        
        // Switch to new screen and hide current one
        private void SwitchTo(IUIScreen newScreen)
        {
            currentScreen?.Hide();
            currentScreen = newScreen;
            currentScreen.Show();
            
            // Only call UpdateUI if it's not an AllInOneScreen - which handles its own setup!!
            if (!(newScreen is AllInOneScreen))
            {
                currentScreen.UpdateUI();
            }
        }
    }


}