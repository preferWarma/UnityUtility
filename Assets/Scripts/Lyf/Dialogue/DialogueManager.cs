using System.Linq;
using DG.Tweening;
using Lyf.Utils.Extension;
using Lyf.Utils.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace Lyf.Dialogue
{
    public class DialogueManager :　GlobalSingleton<DialogueManager>
    {
        [Header("数据集")]
        [Label("对话人物图片集")] public Sprite[] sprites; // 对话人物图片集
        [Label("对话人物名字集")] public string[] names; // 对话人物名字集
        
        [Header("位置集")]
        [Label("图片显示位置集(上中下)")] public RectTransform[] iconPositions; // 图片显示位置(分别为左中右三边)
        [Label("名字显示位置")] public RectTransform namePosition; // 名字显示位置
        
        [Header("UI组件")]
        [Label("文本显示组件")] public Text mainText;  // 文本
        [Label("对话面板对象Panel")] public GameObject panel;   // 对话面板对象
        [Label("选项生成位置(Panel)")] public RectTransform optionPanel;  // 选项相关panel
        [Label("选项Prefabs")] public Button optionPrefab;  // 选项的预制件
        [Label("next按钮")] public Button nextButton;  // next按钮
        
        [Header("对话信息")]
        [Label("当前对话数据")] public DialogueData currentDialogueData;    // 当前对话数据
        [Label("当前的Piece信息")] public PieceData currentPieceData; // 当前Piece信息
        
        [Header("其他设置")]
        [Label("文字显示速度(个/s)")] public float textSpeed = 10f; // 文字显示速度

        private void Start()
        {
            nextButton.onClick.AddListener(ContinueDialogue);  // 绑定next按钮
        }

        public void SetDialogueData(DialogueData other)  // 设置对话数据
        {
            currentDialogueData = other;
            currentPieceData = currentDialogueData.dataList[0];  // 保证改变对话数据集后每次都是从第一条对话开始
            UpdateShow();  // 更新为当前对话显示
        }
        
        private void UpdateShow() // 更新为当前对话显示
        {
            panel.SetActive(true);  // 启动UI
            
            UpdateName();  // 更新名字
            UpdateSprite();  // 更新图片
            UpdateText();  // 更新文本
            UpdateOptions();  // 更新选项
        }

        private void UpdateOptions()    // 更新选项显示
        {
            if (currentPieceData.type == TypeEnum.SingleWithOption) // 如果是有选项的对话
                CreateOptions(currentPieceData);
            else 
            {
                foreach (Transform child in optionPanel) // 如果没有选项则删除原有选项
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void UpdateText()   // 更新文本
        {
            mainText.DOText(currentPieceData.detail, currentPieceData.detail.Length / textSpeed).From("");  // 显示文本，速度为0.1s/字
        }

        private void UpdateName()   // 更新名字
        {
            // 如果有名字则显示，没有设置为空
            namePosition.gameObject.GetComponent<Text>().text = currentPieceData.figureID != -1 ? names[currentPieceData.spriteID - 1] : "";
        }

        private void UpdateSprite() // 更新图片
        {
            if (currentPieceData.spriteID != -1)    // 如果有图片
            {
                var sprite = sprites[currentPieceData.spriteID];  // 获取
                var locationEnum = currentPieceData.figureLocation;  // 获取图片位置类型
                var index = 0;
                if (locationEnum == FigureLocationEnum.L) index = 0;
                else if (locationEnum == FigureLocationEnum.M) index = 1;
                else if (locationEnum == FigureLocationEnum.R) index = 2;
                var showTransform = iconPositions[index];  // 获取图片位置
                showTransform.gameObject.SetActive(true);   // 显示图片对象
                for (var i = 0; i < iconPositions.Length; i++)  // 隐藏其他图片
                {
                    if (i == index) continue;
                    iconPositions[i].gameObject.SetActive(false);
                }
                var image = showTransform.GetComponent<Image>();  // 获取图片组件
                image.sprite = sprite;  // 设置图片
                showTransform.GetChild(0).GetComponent<Image>().sprite = sprite;
            }
            else
            {
                foreach (var iconPosition in iconPositions)
                {
                    iconPosition.gameObject.SetActive(false);
                }
            }
        }
        
        private void CreateOptions(PieceData pieceData) // 创建当前Piece选项
        {
            // 先删除原有的选项
            foreach (Transform child in optionPanel)
            {
                Destroy(child.gameObject);
            }
            // 创建新的选项
            foreach (var idx in pieceData.nextID)
            {
                var option = Instantiate(optionPrefab, optionPanel);  // 实例化选项
                var optionPiece = GetPieceData(idx);  // 获取选项Piece
                var optionText = optionPiece.detail;  // 获取选项文本
                option.GetComponentInChildren<Text>().text = optionText;  // 设置选项文本
                var index = optionPiece.nextID[0];  // 获取选项对应的下一条对话ID
                option.onClick.AddListener(() => { ContinueDialogue(index); });  // 绑定选项点击事件
            }
        }
        
        private void ContinueDialogue(int index)    // 继续对话
        {
            if (index == -1)  // 如果没有下一条对话
            {
                panel.SetActive(false);  // 关闭对话面板
            }
            else // 如果有下一条对话
            {
                currentPieceData = GetPieceData(index);  // 获取下一条对话的PieceData
                UpdateShow();  // 更新显示
            }
        }

        private void ContinueDialogue() // 继续对话
        {
            if (currentPieceData.type != TypeEnum.Single) return;
            
            var nextPieceID = currentPieceData.nextID;  // 获取下一条对话的ID
            ContinueDialogue(nextPieceID[0]);
        }
        
        private PieceData GetPieceData(int id)   // 获取PieceData
        {
            return currentDialogueData.dataList.FirstOrDefault(pieceData => pieceData.id == id);
        }
    }
}