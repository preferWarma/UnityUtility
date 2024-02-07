using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lyf.ExcelKit
{
    public class ScriptableDataGenerate : MonoBehaviour
    {
        /// <summary>
        /// 将DataTable转换为List类型
        /// </summary>
        /// <param name="dataTable">表格数据</param>
        /// <typeparam name="T">表示每一行的类</typeparam>
        /// <returns></returns>
        public static List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
        {
            var rowDataDataList = new List<T>();
            for (var i = 3; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                
                var rowData = new T();
                var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (var j = 0; j < fields.Length; j++)
                {
                    var field = fields[j];
                    var fieldName = field.Name; // 字段名
                    var fieldType = field.FieldType;    // 字段类型
                    var fieldValue = row[j];    // 字段值
                    
                    if (fieldValue == DBNull.Value) continue;
                    
                    if (fieldType == typeof(int))
                    {
                        field.SetValue(rowData, Convert.ToInt32(fieldValue));
                    }
                    else if (fieldType == typeof(string))
                    {
                        field.SetValue(rowData, fieldValue.ToString());
                    }
                    else if (fieldType.IsEnum)
                    {
                        if (Enum.TryParse(field.FieldType, fieldValue.ToString(), out var result))
                        {
                            field.SetValue(rowData, result);
                        }
                        else
                        {
                            Debug.LogError($"{fieldType}转换失败");
                        }
                    }
                    else if (fieldType == typeof(GameObject))
                    {
                        field.SetValue(rowData, Resources.Load<GameObject>(fieldValue.ToString()));
                    }
                    else if (fieldType == typeof(int[]))
                    {
                        field.SetValue(rowData, Array.ConvertAll(fieldValue.ToString().Split(','), int.Parse));
                    }
                    else if (fieldType == typeof(string[]))
                    {
                        field.SetValue(rowData, fieldValue.ToString().Split(','));
                    }
                    else if (fieldType == typeof(float[]))
                    {
                        field.SetValue(rowData, Array.ConvertAll(fieldValue.ToString().Split(','), float.Parse));
                    }
                }
                
                rowDataDataList.Add(rowData);
            }

            return rowDataDataList;
        }
        
        /// <summary>
        /// 通过Excel数据生成多个ScriptableObject数据
        /// </summary>
        /// <param name="filePath">Excel文件读取路径</param>
        /// <param name="assetPath">ScriptableObject存放路径</param>
        // ReSharper disable once UnusedMember.Global, 用于反射调用
        public static void GenerateScriptableObject<T1, T2>(string filePath, string assetPath) where T1 : new() where T2 : ScriptableObject
        {
            var sheets = ExcelHelper.ReadExcelToRowClassList<T1>(filePath);

            foreach (var (sheetName, dataList) in sheets)
            {
                // 生成ScriptableObject实例
                var myScriptableObject = ScriptableObject.CreateInstance<T2>();
                // 只获取当前类的字段(非静态), 不获取父类的字段
                var field = typeof(T2).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                field[0].SetValue(myScriptableObject, dataList);
                myScriptableObject.name = sheetName;

                // 保存ScriptableObject
                var path = assetPath + "/" + myScriptableObject.name + ".asset";
                CreateFoldersIfNotExist(path); // 如果文件夹不存在则递归创建文件夹
                AssetDatabase.CreateAsset(myScriptableObject, path); // 在path下创建资源
            }

            AssetDatabase.SaveAssets(); //保存
        }

        private static void CreateFoldersIfNotExist(string path)    // 递归创建文件夹
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath)) return;
            if (directoryPath != null) Directory.CreateDirectory(directoryPath);
            else throw new Exception("路径为空");
        }
        
        /// <summary>
        /// 修改OpenWindow.Designer.cs文件中的Execute方法, 使其适应新生成的类名
        /// </summary>
        /// <param name="rowClassName">表示一行数据的类的名字</param>
        /// <param name="tableClassName">表示整个表的数据的类的名字(ScriptableObject类型)</param>
        public static void ModifyExecuteMethod(string rowClassName, string tableClassName)
        {
            try
            {
                // 读取Template_OpenWindow_Designer.txt文件
                const string filePath = "Assets/Scripts/Lyf/ExcelKit/Editor/Template_OpenWindow_Designer.txt";
                var fileContent = File.ReadAllText(filePath);

                // 将文件内容部分替换
                var replaceContent = fileContent.Replace("@1", rowClassName)
                    .Replace("@2", tableClassName);

                // 写入文件
                const string savePath = "Assets/Scripts/Lyf/ExcelKit/Editor/OpenWindow.Designer.cs";
                File.WriteAllText(savePath, replaceContent);
                
                Debug.Log("修改成功");
            }
            catch (Exception e)
            {
                Debug.LogError("修改失败");
                Debug.LogError(e);
            }
        }
        
        /// <summary>
        /// 恢复被修改的代码文件
        /// </summary>
        [MenuItem("Lyf/ExcelKit/恢复被修改的代码文件")]
        public static void RestoreCodeFile()
        {
            try 
            {
                // 读取Template_OpenWindow_Designer.txt文件
                const string filePath = "Assets/Scripts/Lyf/ExcelKit/Editor/Template_OpenWindow_Designer.txt";
                var fileContent = File.ReadAllText(filePath);

                // 将文件内容部分替换
                var replaceContent = fileContent.Replace("@1", "void").Replace("@2", "void");
            
                // 写入文件
                const string savePath = "Assets/Scripts/Lyf/ExcelKit/Editor/OpenWindow.Designer.cs";
            
                File.WriteAllText(savePath, replaceContent);
                
                Debug.Log("恢复成功");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError("恢复失败");
                Debug.LogError(e);
            }
        }
    }
}