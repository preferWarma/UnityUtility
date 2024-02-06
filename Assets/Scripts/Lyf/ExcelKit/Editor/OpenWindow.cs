using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Lyf.ExcelKit.Editor
{
    public partial class OpenWindow : EditorWindow
    {
        private string _assetPath = "Assets/SO_Data/DialogueData";
        private string _filePath = "Assets/ExcelFiles/对话表格.xlsx";
        
        [MenuItem("Lyf/Generate DialogueData")]
        public static void ShowWindow()
        {
            GetWindow<OpenWindow>("Generate DialogueData Window");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("请填写Excel读取路径和数据路径: ");
            _assetPath = EditorGUILayout.TextField("数据保存路径: ", _assetPath);
            _filePath = EditorGUILayout.TextField("Excel文件路径: ", _filePath);

            if (GUILayout.Button("生成代码"))
            {
                var excelData = ExcelHelper.ReadExcel(_filePath);
                // 获取第一个表格的数据
                var dataTable = excelData.First().Value;
                var classNames = ExcelHelper.GenerateClassAndEnumCode(excelData.First().Value, "Assets/Scripts/Generation", 
                    "Test_RowClass", "Test_TableClass", "Generation");
                // 修改生成数据的代码
                ScriptableDataGenerate.ModifyExecuteMethod(classNames[0], classNames[1]);
                AssetDatabase.Refresh();
            }
            
            if (GUILayout.Button("生成数据"))
            {
                // // 路径为空, 提示错误
                // if (string.IsNullOrEmpty(_assetPath) || string.IsNullOrEmpty(_filePath))
                // {
                //     Debug.LogError("路径不能为空");
                //     return;
                // }
                //
                // // 若原文件夹已经存在, 进行二次确认
                // if (Directory.Exists(_assetPath))
                // {
                //     if (!EditorUtility.DisplayDialog("确认生成", "原文件夹已经存在, 该选择可能会覆盖原有的数据文件, 是否确定?", "Yes", "No"))
                //     {
                //         return;
                //     }
                // }
                
                // 生成数据
                // DialogueDataGenerate.GenerateScriptableObject<PieceData, DialogueData>(_filePath, _assetPath);
                GenerateScriptableData();
            }
        }
    }
}
#endif