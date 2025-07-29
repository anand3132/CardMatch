using UnityEngine;

namespace CardMatch
{
    /// <summary>
    /// Common interface for all managers in the game.
    /// Provides standardized initialization and cleanup methods.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Initialize the manager. Called during game startup.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Clean up the manager. Called during game shutdown.
        /// </summary>
        void Cleanup();
        
        /// <summary>
        /// Check if the manager is properly initialized.
        /// </summary>
        bool IsInitialized { get; }
    }
} 