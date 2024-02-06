using System.Collections.Generic;
using UnityEngine;

namespace Lyf.Dialogue
{
    [CreateAssetMenu(fileName = "NewScriptableObject", menuName = "ScriptableObjects/NewScriptableObject")]
    public class DialogueData : ScriptableObject
    {
        [Tooltip("当前Table的有效数据列表")] public List<PieceData> dataList;
    }
}