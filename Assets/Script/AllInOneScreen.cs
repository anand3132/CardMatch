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
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Button actionButton;

        private AllInOneScreenMode currentMode;
        private System.Action buttonAction;

        public void Initialize() { }
        public void UpdateUI() { }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void Show(AllInOneScreenMode mode, System.Action onClickAction, bool hasProgress = false)
        {
            currentMode = mode;
            buttonAction = onClickAction;
            UpdateUI(hasProgress);
            Show();
        }

        public void UpdateUI(bool hasProgress = false)
        {
            switch (currentMode)
            {
                case AllInOneScreenMode.GameStart:
                    if (hasProgress)
                    {
                        var gameManager = GamePlayManager.Instance;
                        if (gameManager != null && gameManager.IsInitialized)
                        {
                            titleText.text = "Continue Game";
                            contentText.text = $"Level {gameManager.CurrentLevel} - Score: {gameManager.TotalScore}";
                        }
                        else
                        {
                            titleText.text = "Continue Game";
                            contentText.text = "Loading saved progress...";
                        }
                        actionButton.GetComponentInChildren<TMP_Text>().text = "Continue";
                    }
                    else
                    {
                        titleText.text = "My Awesome Game";
                        contentText.text = "Get ready to play!";
                        actionButton.GetComponentInChildren<TMP_Text>().text = "Start Game";
                    }
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