using UnityEngine;
using UnityEngine.UI;

namespace SaveSystem
{
    public class UIController : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerLevelText;
        [SerializeField] private Text playerScoreText;
        [SerializeField] private Text playerPositionXText;
        [SerializeField] private Text playerPositionYText;
        [SerializeField] private Text playerPositionZText;
        
        [Header("--Player Data")]
        [SerializeField] private Player player;

        [Header("--Button--")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private void Start()
        {
            saveButton.onClick.AddListener(player.SaveData);
            loadButton.onClick.AddListener(player.LoadData);
        }

        private void Update()
        {
            playerNameText.text = player.Name;
            playerLevelText.text = player.Level.ToString();
            playerScoreText.text = player.Score.ToString();
            playerPositionXText.text = player.Position.x.ToString("0.00");
            playerPositionYText.text = player.Position.y.ToString("0.00");
            playerPositionZText.text = player.Position.z.ToString("0.00");
        }
    }
}