using UnityEngine;

namespace CardMatch
{
    // Represents different UI contexts/states that screens can handle
    public enum UIContext
    {
        GameStart,
        GameWon,
        GameLost,
        GamePaused,
        Settings
    }
    
    // Data structure to pass context information to screens
    public class UIContextData
    {
        public UIContext context;
        public int currentLevel;
        public int currentScore;
        public int remainingTurns;
        public System.Action onContinue;
        public System.Action onRestart;
        public System.Action onNextLevel;
        public System.Action onSettings;
        
        public UIContextData(UIContext context)
        {
            this.context = context;
        }
        
        public static UIContextData CreateGameStart()
        {
            var data = new UIContextData(UIContext.GameStart);
            
            if (GamePlayManager.Instance.IsInitialized)
            {
                data.currentLevel = GamePlayManager.Instance.CurrentLevel;
            }
            
            return data;
        }
        
        public static UIContextData CreateGameWon()
        {
            var data = new UIContextData(UIContext.GameWon);
            
            if (GamePlayManager.Instance.IsInitialized)
            {
                data.currentLevel = GamePlayManager.Instance.CurrentLevel;
                data.currentScore = GamePlayManager.Instance.CurrentScore;
            }
            
            return data;
        }
        
        public static UIContextData CreateGameLost()
        {
            var data = new UIContextData(UIContext.GameLost);
            
            if (GamePlayManager.Instance.IsInitialized)
            {
                data.currentLevel = GamePlayManager.Instance.CurrentLevel;
                data.remainingTurns = GamePlayManager.Instance.RemainingTurns;
            }
            
            return data;
        }
    }
} 