using System.Collections.Generic;

namespace CardMatch
{
    // Manages level-specific data like symbol generation and shuffling
    public class LevelManager
    {
        private const string availableSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Generate pairs of symbols for matching game
        public List<string> GenerateSymbolPairs(int pairCount)
        {
            List<string> symbolsList = new List<string>();
            for (int i = 0; i < pairCount; i++)
            {
                string symbol = availableSymbols[i % availableSymbols.Length].ToString();
                symbolsList.Add(symbol);
                symbolsList.Add(symbol);
            }
            return symbolsList;
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