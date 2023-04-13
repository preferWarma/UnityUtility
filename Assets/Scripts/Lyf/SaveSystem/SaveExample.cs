using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace Lyf.SaveSystem
{
    public class PlayerData
    {
        public string PlayName;
        public int Level;
        public int Score;
        public Vector3 Position;
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
            SaveManager.Instance.SaveAllRegister(SaveType.Json);
            // 或者 SaveManager.Save(this, SaveType.Json);
        }

        public void LoadData()
        {
            SaveManager.Instance.LoadAllRegister(SaveType.Json);
            // 或者 SaveManager.Load(this, SaveType.Json);
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
        }

        public void LoadWithPlayerPrefs()
        {
            var saveData = SaveManager.LoadWithPlayerPrefs<PlayerData>(SAVE_KEY);
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