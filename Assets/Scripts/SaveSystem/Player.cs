using UnityEditor;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

namespace SaveSystem
{
    public class PlayerData
    {
        public string PlayName = "Player";
        public int Level = 1;
        public int Score = 0;
        public Vector3 Position = Vector3.zero;
    }
    
    public class Player : MonoBehaviour,ISaveWithJson,ISaveWithPlayerPrefs
    {
        [SerializeField] public string playName = "Player";
        [SerializeField] public int level = 1;
        [SerializeField] public int score = 0;

        public string Name => playName;
        public int Level => level;
        public int Score => score;
        public Vector3 Position => transform.position;
        
        public void SaveData()
        {
            // SaveWithPlayerPrefs();
            SaveWithJson();
        }
        
        public void LoadData()
        {
            // LoadWithPlayerPrefs();
            LoadWithJson();
        }

        #region Use PlayerPrefs

        public string SAVE_KEY => "PlayerData";

        public void SaveWithPlayerPrefs()
        {
            var saveData = new PlayerData()
            {
                PlayName = playName,
                Level = level,
                Score = score,
                Position = transform.position
            };
            SaveManager.SaveWithPlayerPrefs(SAVE_KEY, saveData);
            PlayerPrefs.Save();
        }

        public void LoadWithPlayerPrefs()
        {
            var saveData = JsonUtility.FromJson<PlayerData>(SaveManager.LoadWithPlayerPrefs(SAVE_KEY));
            playName = saveData.PlayName;
            level = saveData.Level;
            score = saveData.Score;
            transform.position = saveData.Position;
        }
        
        #endregion

        #region Use Json

        public string SAVE_FILE_NAME => "PlayerData.json";

        public void SaveWithJson()
        {
            var saveData = new PlayerData()
            {
                PlayName = playName,
                Level = level,
                Score = score,
                Position = transform.position
            };
            SaveManager.SaveWithJson(SAVE_FILE_NAME, saveData);
        }

        public void LoadWithJson()
        {
            var saveData = SaveManager.LoadWithJson<PlayerData>(SAVE_FILE_NAME);
            playName = saveData.PlayName;
            level = saveData.Level;
            score = saveData.Score;
            transform.position = saveData.Position;
        }
        
        #endregion
        
    }
}