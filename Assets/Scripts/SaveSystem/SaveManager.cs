using System.IO;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace SaveSystem
{
    public interface ISaveable { }
    
    public interface ISaveWithPlayerPrefs : ISaveable
    {
        string SAVE_KEY { get; }
        void SaveWithPlayerPrefs();
        void LoadWithPlayerPrefs();
    }
    
    public interface ISaveWithJson : ISaveable
    {
        string SAVE_FILE_NAME { get; }
        void SaveWithJson();
        void LoadWithJson();
    }

    public static class SaveManager
    {
        #region Use PlayerPrefs
        public static void SaveWithPlayerPrefs(string key ,object data)
        {
            var jsonData = JsonUtility.ToJson(data, true);
            Debug.Log(jsonData);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }
        public static string LoadWithPlayerPrefs(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        
        #endregion

        #region Use Json

        public static void SaveWithJson(string saveFileName, object data)
        {
            var jsonData = JsonUtility.ToJson(data, true);
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            File.WriteAllText(path, jsonData);
#if UNITY_EDITOR
            Debug.Log("Save data to " + path);
#endif
        }
        
        public static T LoadWithJson<T>(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            var jsonData = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(jsonData);
            return data;
        }

        public static void DeleteSaveFile(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            File.Delete(path);
        }
        
        #endregion
    }
    
    
}