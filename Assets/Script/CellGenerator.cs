using UnityEngine;
using System.Collections.Generic;
namespace CardMatch
{
    public class CellGenerator
    {
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

            return cells;
        }
    }

}