using UnityEngine;

namespace CardMatch
{
    // Central application entry point that manages all game systems.
    public class ApplicationEntry : MonoBehaviour
    {
        [Header("Manager References")]
        [SerializeField] private GamePlayManager gamePlayManager;
        [SerializeField] private UXManager uxManager;
        
        void Awake() => FindManagers();
        
        void Start() => InitializeAllManagers();
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && SaveSystem.GameData != null)
            {
                // Save cell states and game data when app is paused
                var gameManager = GamePlayManager.Instance;
                if (gameManager != null && gameManager.IsInitialized)
                {
                    CellSystem.SaveCellStates(gameManager.GetCurrentPanels());
                }
                SaveSystem.Save();
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && SaveSystem.GameData != null)
            {
                // Save cell states and game data when app loses focus
                var gameManager = GamePlayManager.Instance;
                if (gameManager != null && gameManager.IsInitialized)
                {
                    CellSystem.SaveCellStates(gameManager.GetCurrentPanels());
                }
                SaveSystem.Save();
            }
        }
        
        void OnApplicationQuit()
        {
            CleanupAllManagers();
            SaveSystem.Save();
        }
        
        private void FindManagers()
        {
            // Find managers if not assigned in inspector
            if (gamePlayManager == null)
                gamePlayManager = FindObjectOfType<GamePlayManager>();
                
            if (uxManager == null)
                uxManager = FindObjectOfType<UXManager>();
        }
        
        private void InitializeAllManagers()
        {
            // Initialize SaveSystem first
           SaveSystem.Initialize();
           // Initialize managers in order
           InitializeManager(gamePlayManager);
           InitializeManager(uxManager);
           
           // Start the game flow
           StartGameFlow();
        }
        
        private void InitializeManager(IManager manager)
        {
            if (manager != null)
            {
                manager.Initialize();
                
                if (!manager.IsInitialized)
                {
                    Debug.LogError($"Application Entry: {manager.GetType().Name} failed to initialize!");
                }
            }
            else
            {
                Debug.LogError($"Application Entry: Manager not found!");
            }
        }
        
        private void StartGameFlow()
        {
            // Let UXManager handle the initial game flow
            if (uxManager != null && uxManager.IsInitialized)
            {
                uxManager.ShowGameStart();
            }
            else
            {
                Debug.LogError("Application Entry: UXManager not available for game flow!");
            }
        }
        
        private void CleanupAllManagers()
        {
            // Clean up in reverse order
            CleanupManager(uxManager);
            CleanupManager(gamePlayManager);
        }
        
        private void CleanupManager(IManager manager)
        {
            if (manager != null)
            {
                manager.Cleanup();
            }
        }
    }
} 