using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace CardMatch
{
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

        public void OnPanelClick()
        {
            Debug.Log("OnPanelClick");
            if (isFlipping) return;

            StartCoroutine(FlipPanelCoroutine(isFlipped ? 0 : 180, !isFlipped,
                () => { GamePlayManager.Instance.CurrentMove(this); }));
        }

        public void Initialise(string id)
        {
            marker.text = id;
            cellID = id;
        }

        public void Reset()
        {
            if (isFlipping) return;

            StartCoroutine(FlipPanelCoroutine(0, false, null));
        }
        
        public void SetState(bool isFlipped, bool isMatched = false)
        {
            if (isFlipping) return;
            
            this.isFlipped = isFlipped;
            
            // Set the visual state immediately without animation
            panelTransform.rotation = Quaternion.Euler(0, isFlipped ? 180 : 0, 0);
            frontSide.SetActive(!isFlipped);
            backSide.SetActive(isFlipped);
            
            // If matched, disable interaction
            GetComponent<Button>().interactable = !isMatched;
        }
        
        public CellState GetState(int cellIndex)
        {
            return new CellState
            {
                cellID = cellID,
                isMatched = !GetComponent<Button>().interactable,
                isFlipped = isFlipped,
                cellIndex = cellIndex
            };
        }

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