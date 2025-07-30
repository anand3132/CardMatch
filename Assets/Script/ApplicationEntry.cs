using UnityEngine;

namespace CardMatch
{
    // Central application entry point that manages all game systems lifecycle
    public class ApplicationEntry : MonoBehaviour
    {
        [Header("Manager References")]
        [SerializeField] private GamePlayManager gamePlayManager;
        [SerializeField] private UXManager uxManager;
        [SerializeField] private SoundManager soundManager;
        
        void Awake() => FindManagers();
        
        void Start() => InitializeAllManagers();
        
        // Handle app pause and save game state
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
        
        // Handle app focus loss and save game state
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
        
        // Find and assign manager references if not set in inspector
        private void FindManagers()
        {
            // Find managers if not assigned in inspector
            if (gamePlayManager == null)
                gamePlayManager = FindObjectOfType<GamePlayManager>();
                
            if (uxManager == null)
                uxManager = FindObjectOfType<UXManager>();
                
            if (soundManager == null)
                soundManager = FindObjectOfType<SoundManager>();
        }
        
        // Initialize all managers in proper order
        private void InitializeAllManagers()
        {
            // Initialize SaveSystem first
           SaveSystem.Initialize();
           // Initialize managers in order
           InitializeManager(soundManager);
           InitializeManager(gamePlayManager);
           InitializeManager(uxManager);
           
           // Start the game flow
           StartGameFlow();
        }
        
        // Initialize a single manager and check if successful
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
        
        // Start the initial game flow through UXManager
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
        
        // Clean up all managers in reverse order
        private void CleanupAllManagers()
        {
            // Clean up in reverse order
            CleanupManager(uxManager);
            CleanupManager(gamePlayManager);
            CleanupManager(soundManager);
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