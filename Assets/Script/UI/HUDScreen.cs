using CardMatch;
using UnityEngine;
using TMPro;

// HUD screen displaying score, turns, and level information
public class HUDScreen : MonoBehaviour, IUIScreen
{
    [Header("UI Panel")]
    [SerializeField] private GameObject uiPanel; // The actual UI panel that gets enabled/disabled
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnsText;
    [SerializeField] private TMP_Text levelText;

    private int score;
    private int turns;
    private int level;

    // Initialize HUD panel and set default values
    public void Initialize()
    {
        if (uiPanel != null)
        {
            uiPanel?.SetActive(false);
        }
        else
        {
            Debug.LogError("HUDScreen: UI Panel not assigned! Please assign the UI panel in the inspector.");
        }
        
        score = 0;
        turns = 0;
        level = 1;
        UpdateUI();
    }
    
    private void OnValidate()
    {
        if (uiPanel == null)
        {
            Debug.LogWarning("HUDScreen: UI Panel not assigned! Please assign the UI panel GameObject.");
        }
        
        if (scoreText == null || turnsText == null || levelText == null)
        {
            Debug.LogWarning("HUDScreen: Some UI elements are not assigned! Please check the inspector.");
        }
    }

    public void UpdateUI() => RefreshFromGameManager();

    // Set local data and update UI elements
    private void SetData(int score, int turns, int level)
    {
        this.score = score;
        this.turns = turns;
        this.level = level;
        UpdateUIElements();
    }
    
    private void UpdateUIElements()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (turnsText != null) turnsText.text = $"Turns: {turns}";
        if (levelText != null) levelText.text = $"Level: {level}";
    }
    
    // Show HUD and subscribe to game events
    public void Show() 
    { 
        uiPanel?.SetActive(true);
        RefreshFromGameManager();
        SubscribeToGameEvents();
    }
    
    // Hide HUD and unsubscribe from game events
    public void Hide() 
    { 
       uiPanel?.SetActive(false);
       UnsubscribeFromGameEvents();
    }
    
    // Subscribe to score and turns change events
    private void SubscribeToGameEvents()
    {
        GamePlayManager.Instance.OnScoreChanged += OnScoreChanged;
        GamePlayManager.Instance.OnTurnsChanged += OnTurnsChanged;
    }
    
    // Unsubscribe from score and turns change events
    private void UnsubscribeFromGameEvents()
    {
        GamePlayManager.Instance.OnScoreChanged -= OnScoreChanged;
        GamePlayManager.Instance.OnTurnsChanged -= OnTurnsChanged;
    }
    
    private void OnScoreChanged(int score) => RefreshFromGameManager();
    
    private void OnTurnsChanged(int turns) => RefreshFromGameManager();
    
    public void HandleContext(UIContextData contextData) => RefreshFromGameManager();
        
    // Refresh HUD data from GamePlayManager
    private void RefreshFromGameManager()
    {
        if (GamePlayManager.Instance.IsInitialized)
        {
            // Update local data
            score = GamePlayManager.Instance.CurrentScore;
            turns = GamePlayManager.Instance.RemainingTurns;
            level = GamePlayManager.Instance.CurrentLevel;
            
            // Update UI elements directly
            UpdateUIElements();
        }
        else
        {
            Debug.LogWarning("HUD Refresh failed - GamePlayManager not initialized");
        }
    }
}