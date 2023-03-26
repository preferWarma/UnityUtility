using UnityEngine;

namespace Dialogue
{
    // 基类含有属性currentDialogueData, 表示挂载该脚本的物体要传输对话内容信息
    // 基类含有方法OpenDialogue, 可以使用该脚本中的对话数据开启对话
    public class DialogueController : BaseDialogueController
    {
        private void Update()
        {
            // 若挂载对象名为“NPC1”的时候，按下A键开启对话
            if (gameObject.name == "NPC1" && Input.GetKeyDown(KeyCode.A))
            {
                OpenDialogue();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // 若挂载对象名为“NPC2”的时候，进入触发器开启对话
            if (gameObject.name == "NPC2" && other.CompareTag("Player"))
            {
                OpenDialogue();
            }
        }
    }
}