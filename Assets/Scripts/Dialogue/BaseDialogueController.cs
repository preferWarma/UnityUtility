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
            transform.GetChild(0).gameObject.SetActive(true);   // 打开对话面板
            DialogueManager.Instance.SetDialogueData(currentDialogueData);   // 传输对话内容信息
        }
    }
}