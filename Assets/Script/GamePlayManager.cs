using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GamePlayManager : Singleton<GamePlayManager>
{
    private int counter = 0;
    private Cell lastSelected;
    public List<Cell> panels;  
    private System.Random random;
    public int seed = 0;
    public int movesCount = 0;
    private const string availableSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // Can extend if needed
    public TextMeshProUGUI moveText;
    public int playCount = 0;
    public void CurrentMove(Cell cell)
    {
        movesCount++;
        if(moveText)
            moveText.text = "Moves: " + movesCount;
        if (counter == 0)
        {
            lastSelected = cell;
            counter++;
            return;
        }
        else
        {
            counter = 0;
            if (!string.Equals(lastSelected.cellID, cell.cellID))
            {
                lastSelected.Reset();
                cell.Reset();
            }
            else
            {
                playCount += 2;
                if (playCount >= panels.Count)
                {
                    Debug.Log("Game Over.......You Win..!!");
                    Reset();
                    
                }
            }
            
            lastSelected = null;
        }
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

    void Start()
    {
        Debug.Log("Game Started: please import TextMeshPro Text");
        panels = gameObject.GetComponentsInChildren<Cell>().ToList();
        PopulatePanels();
    }

    void PopulatePanels()
    {
        if (panels.Count % 2 != 0)
        {
            Debug.LogError("Odd number of panels detected! Ensure an even count for pairing.");
            return;
        }

        random = (seed == 0) ? new System.Random() : new System.Random(seed);
        
        int pairCount = panels.Count / 2; // Number of unique symbols needed
        List<string> selectedSymbols = GenerateSymbolPairs(pairCount);

        // Shuffle and assign symbols
        ShuffleList(selectedSymbols);

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].Initialise(selectedSymbols[i]);
        }
    }

    List<string> GenerateSymbolPairs(int pairCount)
    {
        List<string> symbolsList = new List<string>();

        // Get symbols from available letters
        for (int i = 0; i < pairCount; i++)
        {
            string symbol = availableSymbols[i % availableSymbols.Length].ToString();
            symbolsList.Add(symbol);
            symbolsList.Add(symbol); // Create a pair
        }

        return symbolsList;
    }

    void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
