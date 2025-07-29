using System.IO;
using UnityEngine;
namespace CardMatch
{
    public static class SaveSystem
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
        
        // Global static game data
        public static SaveData GameData { get; private set; }

        public static void Initialize()
        {
            GameData = LoadOrInitialize();
        }

        public static void Save()
        {
            if (GameData != null)
            {
                string json = JsonUtility.ToJson(GameData, true);
                File.WriteAllText(SavePath, json);
            }
        }

        public static void Save(SaveData data)
        {
            GameData = data;
            Save();
        }

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