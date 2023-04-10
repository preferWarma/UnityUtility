using UnityEngine;
using UnityEngine.UI;

namespace Lyf.SaveSystem
{
    public class UIController : MonoBehaviour
    {
        [Header("--Text--")]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerLevelText;
        [SerializeField] private Text playerScoreText;
        [SerializeField] private Text playerPositionXText;
        [SerializeField] private Text playerPositionYText;
        [SerializeField] private Text playerPositionZText;
        
        [Header("--Player Data--")]
        [SerializeField] private SaveExample saveExample;

        [Header("--Button--")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private void Start()
        {
            saveButton.onClick.AddListener(saveExample.SaveData);
            loadButton.onClick.AddListener(saveExample.LoadData);
        }

        private void Update()
        {
            playerNameText.text = saveExample.Name;
            playerLevelText.text = saveExample.Level.ToString();
            playerScoreText.text = saveExample.Score.ToString();
            playerPositionXText.text = saveExample.Position.x.ToString("0.00");
            playerPositionYText.text = saveExample.Position.y.ToString("0.00");
            playerPositionZText.text = saveExample.Position.z.ToString("0.00");
        }
    }
}