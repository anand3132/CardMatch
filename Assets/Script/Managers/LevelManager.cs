using System.Collections.Generic;
using UnityEngine;

namespace CardMatch
{
    // Manages level-specific data like symbol generation and shuffling
    public class LevelManager
    {
        // Basic alphabet symbols (26 symbols - enough for levels 1-24)
        private const string basicSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        // Extra symbols for levels beyond 24
        private const string extraSymbols = "1234567890!@#$%^&*()_+-=[]{}|;:,.<>?/";
        
        // Combined symbols for very high levels
        private const string combinedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?/";

        // Generate pairs of symbols for matching game
        public List<string> GenerateSymbolPairs(int pairCount)
        {
            List<string> symbolsList = new List<string>();
            
            // Determine which symbol set to use based on pair count
            string symbolSet = GetSymbolSetForPairCount(pairCount);
            
            for (int i = 0; i < pairCount; i++)
            {
                string symbol = symbolSet[i % symbolSet.Length].ToString();
                symbolsList.Add(symbol);
                symbolsList.Add(symbol);
            }
            
            Debug.Log($"LevelManager: Generated {pairCount} pairs using {symbolSet.Length} symbols");
            return symbolsList;
        }
        
        // Determine which symbol set to use based on the number of pairs needed
        private string GetSymbolSetForPairCount(int pairCount)
        {
            // Level 1-24: 2-26 pairs (4-52 cells) - use basic alphabet
            if (pairCount <= basicSymbols.Length)
            {
                Debug.Log($"LevelManager: Using basic symbols (A-Z) for {pairCount} pairs");
                return basicSymbols;
            }
            
            // Level 25-30: 27-30 pairs (54-60 cells) - use extra symbols
            if (pairCount <= basicSymbols.Length + extraSymbols.Length)
            {
                Debug.Log($"LevelManager: Using extra symbols (1-0, !-?) for {pairCount} pairs");
                return extraSymbols;
            }
            
            // Level 31+: 31+ pairs (62+ cells) - use combined symbols
            Debug.Log($"LevelManager: Using combined symbols for {pairCount} pairs");
            return combinedSymbols;
        }
        
        // Get symbol set based on level number
        public string GetSymbolSetForLevel(int level)
        {
            // Calculate pairs needed for this level
            int cells = 4 + (level - 1) * 2; // Level progression formula
            int pairs = cells / 2;
            
            return GetSymbolSetForPairCount(pairs);
        }
        
        // Get available symbol count for a given level
        public int GetAvailableSymbolCount(int level)
        {
            string symbolSet = GetSymbolSetForLevel(level);
            return symbolSet.Length;
        }

        // Shuffle list using Fisher-Yates algorithm
        public void Shuffle<T>(List<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int index = rng.Next(0, i + 1);
                (list[i], list[index]) = (list[index], list[i]);
            }
        }
    }
}