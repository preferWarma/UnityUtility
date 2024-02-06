using System.IO;
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
        private string _codeSavePath = "Assets/Scripts/Generation";
        private string _rowClassName = "Test_RowClass";
        private string _tableClassName = "Test_TableClass";
        
        [MenuItem("Lyf/ExcelKit/工具包界面")]
        public static void ShowWindow()
        {
            GetWindow<OpenWindow>("Generate DialogueData Window");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("请填写Excel读取路径和数据路径: ");
            
            GUILayout.BeginHorizontal(); // 横向布局, 保证输入框和按钮在同一行
            _assetPath = EditorGUILayout.TextField("数据保存路径: ", _assetPath);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var assetPath = EditorUtility.OpenFolderPanel("选择数据保存路径", _assetPath, "");
                if (!string.IsNullOrEmpty(assetPath))
                {
                    _assetPath = GetRelativePath(assetPath);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(); // 横向布局, 保证输入框和按钮在同一行
            _filePath = EditorGUILayout.TextField("Excel文件路径: ", _filePath);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var filePath = EditorUtility.OpenFilePanel("选择Excel表格", "", "");
                if (!string.IsNullOrEmpty(filePath))
                {
                    _filePath = GetRelativePath(filePath);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10); // 空行, 避免按钮太紧凑
            
            EditorGUILayout.LabelField("请填写生成代码相关信息: ");
            
            GUILayout.BeginHorizontal(); // 横向布局, 保证输入框和按钮在同一行
            _codeSavePath = EditorGUILayout.TextField("代码保存路径: ", _codeSavePath);
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var codeSavePath = EditorUtility.OpenFolderPanel("选择代码保存路径", _codeSavePath, "");
                if (!string.IsNullOrEmpty(codeSavePath))
                {
                    _codeSavePath = GetRelativePath(codeSavePath);
                }
            }
            GUILayout.EndHorizontal();
            
            _rowClassName = EditorGUILayout.TextField("行数据类名: ", _rowClassName);
            _tableClassName = EditorGUILayout.TextField("表格数据类名: ", _tableClassName);

            GUILayout.Space(20); // 空行, 避免按钮太紧凑
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("生成代码"))
            {
                DoButtonGenerateCode();
            }
            
            GUILayout.Space(10); // 空行, 避免按钮太紧凑
            
            if (GUILayout.Button("生成数据"))
            {
                DoButtonGenerateData();
            }
            
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 如果路径中有Assets, 则去掉路径中Assets之前的部分, 改为相对路径
        /// </summary>
        private static string GetRelativePath(string path)
        {
            if (!path.Contains("Assets")) return path;
            
            var index = path.IndexOf("Assets", System.StringComparison.Ordinal);
            return path[index..];

        }

        /// <summary>
        /// 按下生成数据按钮后的操作
        /// </summary>
        private void DoButtonGenerateData()
        {
            try
            {

                // 如果代码文件不存在，则弹窗提示
                if (!File.Exists(Path.Combine(_codeSavePath, _rowClassName + ".cs"))
                    || !File.Exists(Path.Combine(_codeSavePath, _tableClassName + ".cs")))
                {
                    // 如果用户选择生成代码, 则直接生成代码, 否则返回
                    if (EditorUtility.DisplayDialog("错误", "代码未生成，无法直接生成数据文件， 是否立即生成代码？", "生成代码", "取消"))
                    {
                        DoButtonGenerateCode();

                    }

                    return;
                }

                var excelData = ExcelHelper.ReadExcel(_filePath);
                foreach (var (sheetName, _) in excelData)
                {
                    if (File.Exists(Path.Combine(_assetPath, sheetName + ".asset")))
                    {
                        if (!EditorUtility.DisplayDialog("确认生成",
                                $"原数据文件\"{sheetName}.asset\"已经存在, 该选择会覆盖原有的数据文件, 是否确定覆盖?", "Yes", "No"))
                        {
                            return;
                        }
                    }
                }
                GenerateScriptableData();
                if (EditorUtility.DisplayDialog("生成成功", "数据生成成功", "OK"))
                {
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("错误", "生成数据失败", "OK");
            }
        }
        
        /// <summary>
        /// 按下生成代码按钮后的操作
        /// </summary>
        private void DoButtonGenerateCode()
        {
            try
            {

                // 判断源文件代码是否存在
                var rowClassPath = Path.Combine(_codeSavePath, _rowClassName) + ".cs";
                var tableClassPath = Path.Combine(_codeSavePath, _tableClassName) + ".cs";
                if (File.Exists(rowClassPath) || File.Exists(tableClassPath))
                {
                    if (!EditorUtility.DisplayDialog("确认生成", "源文件已经存在, 该选择会覆盖原有的代码文件, 是否确定覆盖?", "Yes", "No"))
                    {
                        return;
                    }
                }

                var excelData = ExcelHelper.ReadExcel(_filePath);

                // 获取第一个表格的数据生成代码
                var classNames = ExcelHelper.GenerateClassAndEnumCode(excelData.First().Value,
                    "Assets/Scripts/Generation",
                    _rowClassName, _tableClassName, "Generation");
                // 修改生成数据的代码
                ScriptableDataGenerate.ModifyExecuteMethod(classNames[0], classNames[1]);
                if (EditorUtility.DisplayDialog("生成成功", "代码生成成功", "OK"))
                {
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("错误", "生成代码失败", "OK");
            }
        }
    }
}
#endif