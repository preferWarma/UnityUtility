using UnityEngine;

namespace Dialogue
{
    public abstract class BaseDialogueController : MonoBehaviour
    {
        [Tooltip("对话数据")] public DialogueData currentDialogueData;    // 当前对话信息

        /// <summary>
        /// 开启对话, 传输当前脚本挂载的对话内容信息
        /// </summary>
        protected void OpenDialogue()
        {
            var dialogueCanvas = FindObjectOfType<DialogueManager>().transform.gameObject; // 获取对话管理器
            var dialoguePanel = dialogueCanvas.transform.Find("DialoguePanel");    // 获取对话面板
            if (!dialoguePanel)
                throw new MissingReferenceException("找不到对话面板");
            dialoguePanel.gameObject.SetActive(true);   // 打开对话面板
            DialogueManager.Instance.SetDialogueData(currentDialogueData);   // 传输对话内容信息
        }
    }
}