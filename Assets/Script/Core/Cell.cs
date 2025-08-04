using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace CardMatch
{
    // Individual card cell with flip animation and state management
    public class Cell : MonoBehaviour
    {
        public RectTransform panelTransform;
        public GameObject frontSide;
        public GameObject backSide;
        public float flipDuration = 0.5f;
        public TextMeshProUGUI marker;
        public string cellID;

        private bool isFlipped = false;
        private bool isFlipping = false;
        private bool isMatched = false;

        // Handle card click and trigger flip animation
        public void OnPanelClick()
        {
            if (isFlipping) return;

            // Play card flip sound when card is clicked
            SoundManager.Instance?.PlayCardFlip();

            StartCoroutine(FlipPanelCoroutine(isFlipped ? 0 : 180, !isFlipped,
                () => { GamePlayManager.Instance.CurrentMove(this); }));
        }

        public void Initialise(string id)
        {
            marker.text = id;
            cellID = id;
        }

        // Reset cell to unflipped state with animation
        public void Reset()
        {
            if (isFlipping) return;

            isMatched = false;
            StartCoroutine(FlipPanelCoroutine(0, false, null));
        }
        
        // Set cell visual state immediately without animation
        public void SetState(bool isFlipped, bool isMatched = false)
        {
            if (isFlipping) return;
            
            this.isFlipped = isFlipped;
            this.isMatched = isMatched;
            
            // Set the visual state immediately without animation
            panelTransform.rotation = Quaternion.Euler(0, isFlipped ? 180 : 0, 0);
            frontSide.SetActive(!isFlipped);
            backSide.SetActive(isFlipped);
            
            // If matched, disable interaction
            GetComponent<Button>().interactable = !isMatched;
        }
        
        public CellState GetState(int cellIndex) => new CellState
        {
            cellID = cellID,
            isMatched = isMatched,
            isFlipped = isFlipped,
            cellIndex = cellIndex
        };

        // Animate card flip with smooth rotation
        private IEnumerator FlipPanelCoroutine(float targetRotation, bool flipState, System.Action onComplete)
        {
            isFlipping = true;
            GetComponent<Button>().interactable = !flipState;

            float elapsedTime = 0f;
            Quaternion startRotation = panelTransform.rotation;
            Quaternion endRotation = Quaternion.Euler(0, targetRotation, 0);

            while (elapsedTime < flipDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / flipDuration);
                panelTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
                yield return null;
            }

            panelTransform.rotation = endRotation;

            frontSide.SetActive(!flipState);
            backSide.SetActive(flipState);

            isFlipped = flipState;
            isFlipping = false;

            onComplete?.Invoke();
        }
    }
}