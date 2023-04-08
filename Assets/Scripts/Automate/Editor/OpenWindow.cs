using UnityEditor;
using UnityEngine;

namespace Automate.Editor
{
    public class OpenWindow : EditorWindow
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
            EditorGUILayout.LabelField("Fill in the paths: ");
            _assetPath = EditorGUILayout.TextField("数据保存路径: ", _assetPath);
            _filePath = EditorGUILayout.TextField("Excel文件路径: ", _filePath);
            
            if (GUILayout.Button("生成数据"))
            {
                if (string.IsNullOrEmpty(_assetPath) || string.IsNullOrEmpty(_filePath))
                {
                    Debug.LogError("路径不能为空");
                    return;
                }
                if (!EditorUtility.DisplayDialog("确认生成", "该选择可能会覆盖原有的数据文件, 是否确定?", "Yes", "No"))
                {
                    return; // If the user selects "No", do nothing and return
                }
                DialogueDataGenerate.GenerateScriptableObject(_filePath, _assetPath);
            }
        }
    }
}