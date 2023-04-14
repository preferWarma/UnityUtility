using System.Collections.Generic;
using System.IO;
using Lyf.Utils.Singleton;
using UnityEngine;

namespace Lyf.SaveSystem
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

    public class SaveManager : Singleton<SaveManager>
    {
        private readonly List<ISaveWithPlayerPrefs> _prefs = new();
        private readonly List<ISaveWithJson> _jsons = new();
        
        #region Save and Load
        public static void Save(ISaveable saveable, SaveType saveType)
        {
            if (saveType == SaveType.PlayerPrefs)
            {
                (saveable as ISaveWithPlayerPrefs)?.SaveWithPlayerPrefs();
            }

            else if (saveType == SaveType.Json)
            {
                (saveable as ISaveWithJson)?.SaveWithJson();
            }
        }

        public void SaveAllRegister(SaveType saveType)
        {
            var flag = false;
            var saveNum = 0;
            
            if (saveType == SaveType.PlayerPrefs)
            {
                foreach (var pref in _prefs)
                {
                    Save(pref, SaveType.PlayerPrefs);
                    flag = true;
                    saveNum++;
                }
            }
            else if (saveType == SaveType.Json)
            {
                foreach (var json in _jsons)
                {
                    Save(json, SaveType.Json);
                    flag = true;
                    saveNum++;
                }
            }
            Debug.Log(!flag ? $"保存{saveNum}条数据, 当前没有可保存的数据" : $"成功保存{saveNum}条数据");
        }

        public static void Load(ISaveable saveable, SaveType saveType)
        {
            if (saveType == SaveType.PlayerPrefs)
            {
                (saveable as ISaveWithPlayerPrefs)?.LoadWithPlayerPrefs();
            }

            else if (saveType == SaveType.Json)
            {
                (saveable as ISaveWithJson)?.LoadWithJson();
            }
        }
        
        public void LoadAllRegister(SaveType saveType)
        {
            var flag = false;
            var loadNum = 0;
            
            if (saveType == SaveType.PlayerPrefs)
            {
                foreach (var pref in _prefs)
                {
                    Load(pref, SaveType.PlayerPrefs);
                    flag = true;
                    loadNum++;
                }
            }
            else if (saveType == SaveType.Json)
            {
                foreach (var json in _jsons)
                {
                    Load(json, SaveType.Json);
                    flag = true;
                    loadNum++;
                }
            }
            Debug.Log(!flag ? $"加载{loadNum}条数据, 当前没有可加载的数据" : $"成功加载{loadNum}条数据");
        }
        #endregion
        
        #region Register and UnRegister

        public void Register(ISaveable saveable, SaveType type)
        {
            if (type == SaveType.PlayerPrefs)
            {
                if (saveable is not ISaveWithPlayerPrefs)
                {
                    Debug.LogError(" 保存类型错误，当前类不是 ISaveWithPlayerPrefs 类型, 无法注册");
                }
                _prefs.Add(saveable as ISaveWithPlayerPrefs);
            }
            else if (type == SaveType.Json)
            {
                if (saveable is not ISaveWithJson)
                {
                    Debug.LogError(" 保存类型错误，当前类不是 ISaveWithJson 类型, 无法注册");
                }
                _jsons.Add(saveable as ISaveWithJson);
            }
        }

        public void UnRegister(ISaveable saveable, SaveType type)
        {
            if (type == SaveType.PlayerPrefs)
            {
                if (saveable is not ISaveWithPlayerPrefs)
                {
                    Debug.LogError(" 取消注册类型错误，当前类不是 ISaveWithPlayerPrefs 类型, 无法取消注册");
                }
                _prefs.Remove(saveable as ISaveWithPlayerPrefs);
            }
            else if (type == SaveType.Json)
            {
                if (saveable is not ISaveWithJson)
                {
                    Debug.LogError(" 取消注册类型错误，当前类不是 ISaveWithJson 类型, 无法取消注册");
                }
                _jsons.Remove(saveable as ISaveWithJson);
            }
        }

        #endregion

        #region Use PlayerPrefs

        public static void SaveWithPlayerPrefs(string key, object data)
        {
            var jsonData = JsonUtility.ToJson(data, true);
            Debug.Log(jsonData);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }

        public static T LoadWithPlayerPrefs<T>(string key)
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
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

    public enum SaveType
    {
        PlayerPrefs,
        Json
    }
}