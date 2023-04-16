using System.Collections.Generic;
using System.IO;
using Lyf.Utils.Singleton;
using UnityEngine;
using Newtonsoft.Json;

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

    public class SaveManager : GlobalSingleton<SaveManager>
    {
        private readonly List<ISaveWithPlayerPrefs> _prefs = new();
        private readonly List<ISaveWithJson> _jsons = new();

        private void Start()
        {
            AddSerializedJson.AddAllConverter();
        }

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
            var jsonData = JsonConvert.SerializeObject(data);   // 使用Newtonsoft.Json库, 支持序列化Dictionary和List
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            Debug.Log("Save data to PlayerPrefs");
#endif
        }

        public static T LoadWithPlayerPrefs<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));  // 使用Newtonsoft.Json库, 支持反序列化Dictionary和List
        }

        #endregion

        #region Use Json

        public static void SaveWithJson(string saveFileName, object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);   // 使用Newtonsoft.Json库, 支持序列化Dictionary和List
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            if (!path.EndsWith(".json"))
                path += ".json";    // 保证文件后缀为json
            File.WriteAllText(path, jsonData);
#if UNITY_EDITOR
            Debug.Log("Save data to " + path);
#endif
        }

        public static T LoadWithJson<T>(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            if (!File.Exists(path)) 
                Debug.LogError("Save file not found in " + path);
            if (!path.EndsWith(".json")) 
                path += ".json";    // 保证文件后缀为json
            
            var jsonData = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(jsonData);  // 使用Newtonsoft.Json库, 支持反序列化Dictionary和List
        }

        public static void DeleteSaveFile(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            if (!path.EndsWith(".json"))
                path += ".json";    // 保证文件后缀为json
            if (File.Exists(path))
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