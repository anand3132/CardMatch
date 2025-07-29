using CardMatch;
using UnityEngine;
using TMPro;

public class HUDScreen : MonoBehaviour, IUIScreen
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnsText;
    [SerializeField] private TMP_Text levelText;

    private int score;
    private int turns;
    private int level;

    public void Initialize()
    {
        score = 0;
        turns = 0;
        level = 1;
        UpdateUI();
    }

    public void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        turnsText.text = $"Turns: {turns}";
        levelText.text = $"Level: {level}";
    }

    public void SetData(int score, int turns, int level)
    {
        this.score = score;
        this.turns = turns;
        this.level = level;
        UpdateUI();
    }

    public void Show() 
    { 
        gameObject.SetActive(true);
        RefreshFromGameManager();
    }
    
    public void Hide() => gameObject.SetActive(false);
    
    public void RefreshFromGameManager()
    {
        var gameManager = GamePlayManager.Instance;
        if (gameManager != null && gameManager.IsInitialized)
        {
            SetData(gameManager.CurrentScore, gameManager.RemainingTurns, gameManager.CurrentLevel);
        }
    }
}