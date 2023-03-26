using Dialogue;
using UnityEditor;
using UnityEngine;

namespace Automate
{
    public class DialogueDataGenerate : MonoBehaviour
    {
        [MenuItem("Tools/Generate allDialogueData")]
        public static void GenerateScriptableObject()
        {
            const string assetPath = "Assets/SO_Data/Dialogue";
            const string filePath = "C:\\Users\\2961884371\\Desktop\\对话表格.xlsx";
            
            var sheets = ExcelReader.LoadPieceDataSheets(filePath);
            for (var i = 0; i < sheets.Count; i++)  // 每个Sheet生成一个ScriptableObject
            {
                var myScriptableObject = ScriptableObject.CreateInstance<DialogueData>();
                myScriptableObject.pieceDataList = sheets[i];
                var path = assetPath + (i+1) + ".asset";
                AssetDatabase.CreateAsset(myScriptableObject, path);    // 在path下创建资源
            }
            AssetDatabase.SaveAssets(); //保存
        }
    }
}