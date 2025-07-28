using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace CardMatch
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        private Cell lastSelected;
        private int counter = 0;

        public TextMeshProUGUI moveText;
        public List<Cell> panels;

        private int movesCount = 0;
        private int playCount = 0;

        public int seed = 0;
        private System.Random random;

        private SaveData saveData;
        private LevelManager levelManager;
        
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cellParent;
        void Start()
        {
            saveData = GameProgressManager.LoadOrInitialize();
            levelManager = new LevelManager();
            random = (seed == 0) ? new System.Random() : new System.Random(seed);

            panels = CellGenerator.GenerateCells(cellPrefab, cellParent, saveData.levelProgress.totalCells);
            PopulatePanels();
        }

        public void CurrentMove(Cell cell)
        {
            movesCount++;
            moveText.text = $"Moves: {movesCount}";

            if (counter == 0)
            {
                lastSelected = cell;
                counter++;
                return;
            }

            counter = 0;

            if (lastSelected.cellID != cell.cellID)
            {
                lastSelected.Reset();
                cell.Reset();
            }
            else
            {
                playCount += 2;
                saveData.levelProgress.matchedCells = playCount;

                if (playCount >= panels.Count)
                {
                    saveData.levelProgress.isCompleted = true;
                    saveData.levelProgress.currentLevel++;
                    saveData.levelProgress.totalCells += 2;
                    saveData.levelProgress.matchedCells = 0;

                    GameProgressManager.Save(saveData);
                    Reset();
                    return;
                }

                GameProgressManager.Save(saveData);
            }

            lastSelected = null;
        }

        public void Reset()
        {
            playCount = 0;
            counter = 0;
            movesCount = 0;
            lastSelected = null;

            foreach (var cell in panels)
            {
                cell.Reset();
            }

            PopulatePanels();
        }

        void PopulatePanels()
        {
            int requiredCells = saveData.levelProgress.totalCells;

            if (requiredCells % 2 != 0)
                requiredCells++;

            if (panels.Count < requiredCells)
            {
                Debug.LogWarning($"Not enough panels in scene. Needed: {requiredCells}, Found: {panels.Count}");
                return;
            }

            panels = panels.Take(requiredCells).ToList();

            if (panels.Count % 2 != 0)
            {
                Debug.LogError("Odd number of panels detected!");
                return;
            }

            int pairCount = panels.Count / 2;
            var symbols = levelManager.GenerateSymbolPairs(pairCount);
            levelManager.Shuffle(symbols, random);

            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Initialise(symbols[i]);
            }
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            if (GUILayout.Button("Delete Save"))
            {
                GameProgressManager.Delete();
            }
        }
#endif
    }
}
