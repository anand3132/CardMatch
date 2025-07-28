using System.IO;
using UnityEngine;
namespace CardMatch
{
    public static class GameProgressManager
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");

        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("Saved progress to: " + SavePath);
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
                Debug.Log("Deleted save file.");
            }
        }
    }
}