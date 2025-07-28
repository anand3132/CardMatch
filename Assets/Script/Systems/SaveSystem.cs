using System.IO;
using UnityEngine;
namespace CardMatch
{
    public static class SaveSystem
    {
        private static string filePath => Path.Combine(Application.persistentDataPath, "save.json");

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log("Game Saved:\n" + json);
        }

        public static SaveData Load()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Game Loaded:\n" + json);
                return data;
            }
            else
            {
                Debug.Log("No save file found. Creating new progress...");
                return new SaveData { levelProgress = new LevelProgressData() };
            }
        }

        public static void DeleteSave()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Save file deleted.");
            }
        }
    }
}
