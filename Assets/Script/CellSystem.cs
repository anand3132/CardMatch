using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace CardMatch
{
    // Static system for managing cell generation, population, and state persistence
    public class CellSystem
    {
        // Static reference to current panels for global access
        private static List<Cell> currentPanels;
        private static Cell lastSelected;
        private static int counter = 0;
        private static bool wasWrongMatch = false;
        
        // Set current panels for internal use
        private static void SetCurrentPanels(List<Cell> panels)
        {
            currentPanels = panels;
        }
        
        // Generate and instantiate cells from prefab
        public static List<Cell> GenerateCells(GameObject prefab, Transform parent, int count)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }

            List<Cell> cells = new List<Cell>();
            for (int i = 0; i < count; i++)
            {
                GameObject go = GameObject.Instantiate(prefab, parent);
                Cell cell = go.GetComponent<Cell>();
                cells.Add(cell);
            }
            
            // Update grid layout after creating cells
            UpdateGridLayout(parent);
            
            // Set current panels for global access
            SetCurrentPanels(cells);

            return cells;
        }
        
        // Reset all cells to unflipped state
        internal static void ResetAllCells()
        {
            currentPanels?.ForEach(cell => cell.Reset());
        }
        
        // Set matched cells state and save to persistence
        private static void HandleCorrectMatch(Cell cell1, Cell cell2)
        {
            cell1.SetState(true, true);
            cell2.SetState(true, true);
            
            // Save cell states after match
            SaveCellStates(currentPanels);
        }
        
        // Handle wrong match by resetting cells
        private static void HandleWrongMatch(Cell cell1, Cell cell2)
        {
            cell1.Reset();
            cell2.Reset();
        }
        
        // Process cell selection and determine match result
        public static bool ProcessCellMove(Cell cell)
        {
            wasWrongMatch = false; 
            
            if (counter == 0)
            {
                lastSelected = cell;
                counter = 1;
                return false; // No match yet
            }
            else
            {
                counter = 0;
                
                if (lastSelected.cellID != cell.cellID)
                {
                    HandleWrongMatch(lastSelected, cell);
                    wasWrongMatch = true; 
                    return false; // Wrong match
                }
                else
                {
                    HandleCorrectMatch(lastSelected, cell);
                    return true; // Correct match
                }
            }
        }
        
        // Reset cell selection state
        public static void ResetCellSelection()
        {
            lastSelected = null;
            counter = 0;
            wasWrongMatch = false;
        }
        
        // Check if last move was a wrong match
        public static bool IsWrongMatch()
        {
            return wasWrongMatch;
        }
        
        // Get current panels for internal use
        internal static List<Cell> GetCurrentPanels() => currentPanels;
        
        // Get total cells count for internal use
        internal static int GetTotalCells() => SaveSystem.GameData?.levelProgress.totalCells ?? 0;
        
        // Populate cells with symbols using saved arrangement or generate new one
        public static void PopulateCells(List<Cell> Cells, LevelManager levelManager, System.Random random, int totalCells)
        {
            int requiredCells = totalCells;
            if (requiredCells % 2 != 0) requiredCells++;

            if (Cells.Count < requiredCells)
            {
                Debug.LogWarning($"Not enough panels in scene. Needed: {requiredCells}, Found: {Cells.Count}");
                return;
            }

            var selectedPanels = Cells.Take(requiredCells).ToList();

            if (selectedPanels.Count % 2 != 0)
            {
                Debug.LogError("Odd number of panels detected!");
                return;
            }

            List<string> symbols;
            
            // Check if we have a saved symbol arrangement
            if (SaveSystem.GameData.levelProgress.symbolArrangement != null && 
                SaveSystem.GameData.levelProgress.symbolArrangement.Count == selectedPanels.Count)
            {
                // Use saved symbol arrangement
                symbols = new List<string>(SaveSystem.GameData.levelProgress.symbolArrangement);
                Debug.Log($"CellSystem: Using saved symbol arrangement for {symbols.Count} cells");
            }
            else
            {
                // Generate new symbol arrangement
                int pairCount = selectedPanels.Count / 2;
                symbols = levelManager.GenerateSymbolPairs(pairCount);
                levelManager.Shuffle(symbols, random);
                
                // Save the symbol arrangement for future use
                SaveSystem.GameData.levelProgress.symbolArrangement = new List<string>(symbols);
                Debug.Log($"CellSystem: Generated and saved new symbol arrangement for {symbols.Count} cells");
            }

            for (int i = 0; i < selectedPanels.Count; i++)
            {
                selectedPanels[i].Initialise(symbols[i]);
            }
        }
        
        // Save only matched cells to persistence
        public static void SaveCellStates(List<Cell> panels)
        {
            if (panels == null) return;
            
            SaveSystem.GameData.levelProgress.cellStates.Clear();
            int matchedCellsCount = 0;
            
            for (int i = 0; i < panels.Count; i++)
            {
                var cellState = panels[i]?.GetState(i);
                // Only save matched cells, not unmatched flipped cells
                if (cellState != null && cellState.isMatched)
                {
                    SaveSystem.GameData.levelProgress.cellStates.Add(cellState);
                    matchedCellsCount++;
                }
            }
            
            Debug.Log($"CellSystem: Saved {matchedCellsCount} matched cells out of {panels.Count} total cells");
        }
        
        // Static method for global access without parameters
        public static void SaveCellStates()
        {
            SaveCellStates(currentPanels);
        }
        
        // Restore matched cells from saved state
        public static void RestoreCellStates(List<Cell> panels)
        {
            // First, reset all cells to unflipped state
            panels.ForEach(cell => cell.SetState(false, false));
            
            // Then restore only matched cells
            if (SaveSystem.GameData.levelProgress.cellStates != null && SaveSystem.GameData.levelProgress.cellStates.Count > 0)
            {
                int restoredCellsCount = 0;
                foreach (var cellState in SaveSystem.GameData.levelProgress.cellStates)
                {
                    if (cellState.cellIndex < panels.Count)
                    {
                        panels[cellState.cellIndex]?.SetState(cellState.isFlipped, cellState.isMatched);
                        restoredCellsCount++;
                    }
                }
                Debug.Log($"CellSystem: Restored {restoredCellsCount} matched cells out of {SaveSystem.GameData.levelProgress.cellStates.Count} saved cells");
            }
            else
            {
                Debug.Log("CellSystem: No saved cell states to restore");
            }
        }
        
        // Update grid layout using FlexibleGridLayout or fallback to GridLayoutGroup
        private static void UpdateGridLayout(Transform parent)
        {
            if (parent == null) return;
            
            // Find FlexibleGridLayout component and force layout update
            var flexibleGrid = parent.GetComponent<FlexibleGridLayout>();
            if (flexibleGrid != null)
            {
                // Force layout rebuild to ensure proper positioning
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent as RectTransform);
                Debug.Log($"CellSystem: Updated FlexibleGridLayout for {parent.childCount} cells");
            }
            else
            {
                // Fallback: try to find GridLayoutGroup and force layout update
                var gridLayout = parent.GetComponent<GridLayoutGroup>();
                if (gridLayout != null)
                {
                    // Force layout update
                    var rectTransform = parent as RectTransform;
                    if (rectTransform != null)
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                        Debug.Log($"CellSystem: Updated GridLayoutGroup fallback for {parent.childCount} cells");
                    }
                }
                else
                {
                    Debug.LogWarning($"CellSystem: No FlexibleGridLayout or GridLayoutGroup found on {parent.name}");
                }
            }
        }
    }
}
