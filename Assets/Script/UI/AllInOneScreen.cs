using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace CardMatch
{
    public enum AllInOneScreenMode
    {
        GameStart,
        GameWon,
        GameLost
    }
    public class AllInOneScreen : MonoBehaviour, IUIScreen
    {
        [Header("UI Panel")]
        [SerializeField] private GameObject uiPanel; 
        
        [Header("UI Elements")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Button actionButton;

        private AllInOneScreenMode currentMode;
        private System.Action buttonAction;

        public void Initialize() 
        {
            if (uiPanel != null)
            {
               uiPanel?.SetActive(false);
            }
            else
            {
                Debug.LogError("AllInOneScreen: UI Panel not assigned! Please assign the UI panel in the inspector.");
            }
            
            if (titleText == null || contentText == null || actionButton == null)
            {
                Debug.LogWarning("AllInOneScreen: Some UI elements are not assigned! Please check the inspector.");
                return;
            }
        }
        
        private void OnValidate()
        {
            if (uiPanel == null)
            {
                Debug.LogWarning("AllInOneScreen: UI Panel not assigned! Please assign the UI panel GameObject.");
            }
            
            if (titleText == null || contentText == null || actionButton == null)
            {
                Debug.LogWarning("AllInOneScreen: Some UI elements are not assigned! Please check the inspector.");
            }
        }

        public void Show() => uiPanel?.SetActive(true);

        public void Hide()
        {
           uiPanel?.SetActive(false);
        }

        public void HandleContext(UIContextData contextData)
        {
            switch (contextData.context)
            {
                case UIContext.GameStart:
                    currentMode = AllInOneScreenMode.GameStart;
                    buttonAction = contextData.onContinue ?? contextData.onNextLevel;
                    UpdateUI();
                    break;
                    
                case UIContext.GameWon:
                    currentMode = AllInOneScreenMode.GameWon;
                    buttonAction = contextData.onNextLevel;
                    UpdateUI();
                    break;
                    
                case UIContext.GameLost:
                    currentMode = AllInOneScreenMode.GameLost;
                    buttonAction = contextData.onRestart;
                    UpdateUI();
                    break;
                    
                default:
                    Debug.LogWarning($"AllInOneScreen: Unhandled context {contextData.context}");
                    break;
            }
            
            Show();
        }

        public void UpdateUI()
        {
            switch (currentMode)
            {
                case AllInOneScreenMode.GameStart:
                    if (GamePlayManager.Instance.IsInitialized)
                    {
                        titleText.text = "Card Match Game";
                        contentText.text = $"Level {GamePlayManager.Instance.CurrentLevel} - Score: {GamePlayManager.Instance.CurrentScore}";
                    }
                    else
                    {
                        titleText.text = "Card Match Game";
                        contentText.text = "Loading game...";
                    }
                    actionButton.GetComponentInChildren<TMP_Text>().text = "Start Game";
                    break;

                case AllInOneScreenMode.GameWon:
                    titleText.text = "You Won!";
                    contentText.text = "Great job! Ready for the next level?";
                    actionButton.GetComponentInChildren<TMP_Text>().text = "Next Level";
                    break;

                case AllInOneScreenMode.GameLost:
                    titleText.text = "Game Over";
                    contentText.text = "Try again to beat the level.";
                    actionButton.GetComponentInChildren<TMP_Text>().text = "Restart";
                    break;
            }

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => buttonAction?.Invoke());
        }
    }
}