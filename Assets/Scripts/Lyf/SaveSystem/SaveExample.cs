using System;
using System.Collections.Generic;
using UnityEngine; 
// ReSharper disable Unity.InefficientPropertyAccess

namespace Lyf.SaveSystem
{
    [Serializable]
    public class PlayerData
    {
        public string playName;
        public List<int> playerList;    // 测试List是否可以序列化(分别为Level, Score)
        public Dictionary<string, Vector3> PlayerDict;  // 测试Dictionary是否可以序列化
    }

    public class SaveExample : MonoBehaviour, ISaveWithJson, ISaveWithPlayerPrefs
    {
        [SerializeField] private string playName = "Player";
        [SerializeField] private int level;
        [SerializeField] private int score;
        
        public string Name => playName;
        public int Level => level;
        public int Score => score;
        public Vector3 Position => transform.position;

        private void Start()
        {
            SaveManager.Instance.Register(this, SaveType.Json);
        }

        public void SaveData()
        {
            SaveManager.Save(this, SaveType.Json);
            // 或者 SaveManager.Save(this, SaveType.PlayerPrefs);
            // 或者SaveManager.Instance.SaveAllRegister(SaveType.Json); (如果注册过)
        }

        public void LoadData()
        {
            SaveManager.Load(this, SaveType.Json);
            // 或者SaveManager.Load(this, SaveType.PlayerPrefs);
            // 或者SaveManager.Instance.LoadAllRegister(SaveType.Json); (如果注册过)
        }

        private void OnDestroy()
        {
            SaveManager.Instance.UnRegister(this, SaveType.Json);
        }

        #region Use PlayerPrefs

        public string SAVE_KEY => "PlayerData";

        public void SaveWithPlayerPrefs()
        {
            var saveData = new PlayerData()
            {
                playName = playName,
                playerList = new List<int> {level, score},
                PlayerDict = new Dictionary<string, Vector3>
                {
                    {playName, transform.position}
                }
            };
            SaveManager.SaveWithPlayerPrefs(SAVE_KEY, saveData);
        }

        public void LoadWithPlayerPrefs()
        {
            var saveData = SaveManager.LoadWithPlayerPrefs<PlayerData>(SAVE_KEY);
            playName = saveData.playName;
            level = saveData.playerList[0];
            score = saveData.playerList[1];
            transform.position = saveData.PlayerDict[playName];
        }
        #endregion

        #region Use Json

        public string SAVE_FILE_NAME => "PlayerData.json";

        public void SaveWithJson()
        {
            var saveData = new PlayerData
            {
                playName = playName,
                playerList = new List<int> {level, score},
                PlayerDict = new Dictionary<string, Vector3>
                {
                    {playName, transform.position}
                }
            };
            SaveManager.SaveWithJson(SAVE_FILE_NAME, saveData);
        }

        public void LoadWithJson()
        {
            var saveData = SaveManager.LoadWithJson<PlayerData>(SAVE_FILE_NAME);
            playName = saveData.playName;
            level = saveData.playerList[0];
            score = saveData.playerList[1];
            transform.position = saveData.PlayerDict[playName];
        }
        #endregion
    }
}