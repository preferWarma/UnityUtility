using System.IO;
using Lyf.Dialogue;
using UnityEditor;
using UnityEngine;

namespace Lyf.Automate
{
    public class DialogueDataGenerate : MonoBehaviour
    {
        /// <summary>
        /// 对外开放生成数据的函数接口, 传入Excel文件路径和ScriptableObject存放路径
        /// </summary>
        /// <param name="filePath"> Excel文件路径 </param>
        /// <param name="assetPath"> ScriptableObject存放路径</param>
        public static void GenerateScriptableObject(string filePath, string assetPath)
        {
            var sheets = ExcelReader.LoadPieceDataSheets(filePath, out var sheetNames); // 获取对话数据和sheet名字
            
            for (var i = 0; i < sheets.Count; i++)  // 每个Sheet生成一个ScriptableObject
            {
                var myScriptableObject = ScriptableObject.CreateInstance<DialogueData>();
                myScriptableObject.pieceDataList = sheets[i];
                myScriptableObject.name = sheetNames[i];
                var path = assetPath + "/" + myScriptableObject.name + ".asset";
                CreateFoldersIfNotExist(path);  // 如果文件夹不存在则递归创建文件夹
                AssetDatabase.CreateAsset(myScriptableObject, path);    // 在path下创建资源
            }
            AssetDatabase.SaveAssets(); //保存
        }

        private static void CreateFoldersIfNotExist(string path)    // 递归创建文件夹
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath)) return;
            if (directoryPath != null) Directory.CreateDirectory(directoryPath);
        }
    }
}