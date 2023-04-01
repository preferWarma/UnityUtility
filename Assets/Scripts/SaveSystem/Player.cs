using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace SaveSystem
{
    public class PlayerData
    {
        public string PlayName;
        public int Level;
        public int Score;
        public Vector3 Position;
    }
    
    public class Player : MonoBehaviour,ISaveWithJson,ISaveWithPlayerPrefs
    {
        [SerializeField] private string playName = "Player";
        [SerializeField] private int level;
        [SerializeField] private int score;

        public string Name => playName;
        public int Level => level;
        public int Score => score;
        public Vector3 Position => transform.position;
        
        public void SaveData()
        {
            SaveManager.Save(this, SaveType.Json);
            // 或 SaveManager.Save(this, SaveType.PlayerPrefabs);
        }
        
        public void LoadData()
        {
            SaveManager.Load(this, SaveType.Json);
            // 或 SaveManager.Load(this, SaveType.PlayerPrefabs);
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