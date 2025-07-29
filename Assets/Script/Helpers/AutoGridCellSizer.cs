using UnityEngine;
using UnityEngine.UI;

namespace CardMatch
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class AutoGridCellSizer : MonoBehaviour
    {
        [Tooltip("Min number of columns allowed")]
        public int minColumnCount = 2;

        [Tooltip("Max number of columns allowed")]
        public int maxColumnCount = 8;

        [Tooltip("Min number of rows allowed")]
        public int minRowCount = 1;

        [Tooltip("Max number of rows allowed")]
        public int maxRowCount = 6;

        [Tooltip("Space between cells")] public float spacing = 10f;

        [Tooltip("Padding around grid (used only when full layout applied)")]
        public float padding = 20f;

        [Tooltip("Threshold under which the grid remains compact and centered")]
        public int compactLayoutThreshold = 9;

        private GridLayoutGroup grid;
        private RectTransform rt;

        void Start()
        {
            grid = GetComponent<GridLayoutGroup>();
            rt = GetComponent<RectTransform>();
        }
        
        public void UpdateGridLayout()
        {
            if (grid == null)
            {
                grid = GetComponent<GridLayoutGroup>();
                rt = GetComponent<RectTransform>();
            }
            
            ResizeGrid();
            // Force layout rebuild to ensure proper positioning
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
        

        void ResizeGrid()
        {
            int totalItems = transform.childCount;

            // Choose layout strategy
            int columnCount, rowCount;
            CalculateRectangularGrid(totalItems, out columnCount, out rowCount);

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columnCount;
            grid.spacing = new Vector2(spacing, spacing);

            float availableWidth = rt.rect.width - ((columnCount - 1) * spacing) - padding;
            float availableHeight = rt.rect.height - ((rowCount - 1) * spacing) - padding;

            float cellWidth = availableWidth / columnCount;
            float cellHeight = availableHeight / rowCount;

            // If item count is small, keep grid compact and centered
            if (totalItems <= compactLayoutThreshold)
            {
                float maxSize = Mathf.Min(cellWidth, cellHeight);
                grid.cellSize = new Vector2(maxSize, maxSize);
                grid.childAlignment = TextAnchor.MiddleCenter;
            }
            else
            {
                grid.cellSize = new Vector2(cellWidth, cellHeight);
                grid.childAlignment = TextAnchor.UpperLeft;
            }

            WarnIfGridNotFilled(totalItems, columnCount);
        }

        void CalculateRectangularGrid(int itemCount, out int columns, out int rows)
        {
            // Try to find best rectangular grid shape: width ≈ height
            int bestColumns = Mathf.Clamp(Mathf.CeilToInt(Mathf.Sqrt(itemCount)), minColumnCount, maxColumnCount);
            int bestRows = Mathf.CeilToInt((float)itemCount / bestColumns);
            bestRows = Mathf.Clamp(bestRows, minRowCount, maxRowCount);

            columns = bestColumns;
            rows = bestRows;
        }

        void WarnIfGridNotFilled(int itemCount, int columnCount)
        {
            int remainder = itemCount % columnCount;
            if (remainder != 0)
            {
                int fillNeeded = columnCount - remainder;
                Debug.LogWarning(
                    $"Grid layout not fully filled: Add {fillNeeded} more item(s) to complete the last row.");
            }
        }
    }
}