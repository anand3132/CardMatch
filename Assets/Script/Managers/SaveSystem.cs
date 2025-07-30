using System.IO;
using UnityEngine;
namespace CardMatch
{
    // Static system for saving and loading game data to/from JSON files
    public static class SaveSystem
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
        
        // Global static game data
        public static SaveData GameData { get; private set; }

        // Initialize save system by loading existing data or creating new
        public static void Initialize()
        {
            GameData = LoadOrInitialize();
        }

        // Save current game data to file
        public static void Save()
        {
            if (GameData != null)
            {
                string json = JsonUtility.ToJson(GameData, true);
                File.WriteAllText(SavePath, json);
            }
        }

        // Save provided data and update global reference
        public static void Save(SaveData data)
        {
            GameData = data;
            Save();
        }

        // Load existing save file or create new data if none exists
        public static SaveData LoadOrInitialize()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }

            var newData = new SaveData();
            Save(newData);
            return newData;
        }

        // Delete save file and reset global data
        public static void Delete()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
            GameData = new SaveData();
        }
    }
}