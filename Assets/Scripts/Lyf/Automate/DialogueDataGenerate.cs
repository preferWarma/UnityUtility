using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Lyf.Dialogue;
using UnityEditor;
using UnityEngine;

namespace Lyf.Automate
{
    public class DialogueDataGenerate : MonoBehaviour
    {
        // 将DataTable转换为List<PieceData>
        public static List<PieceData> ConvertToPieceDataList(DataTable dataTable)
        {
            var pieceDataList = new List<PieceData>();
            for (var i = 1; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                
                // 根据你的Excel表格中的必选项不为空来判断是否跳出循环
                if (row["ID"] == DBNull.Value ||
                    row["Type"] == DBNull.Value ||
                    row["Detail"] == DBNull.Value
                   ) break;
                
                // 生成当前行的PieceData
                var pieceData = new PieceData
                {
                    id = Convert.ToInt32(row["ID"]),
                    type = PieceData.IDMap[Convert.ToInt32(row["Type"])],
                    detail = row["Detail"].ToString(),
                    figureID = row["FigureID"] != DBNull.Value ? Convert.ToInt32(row["FigureID"].ToString()[1..]) :　-1,
                    figureLocation =　row["FigureLocation"] != DBNull.Value　
                        ? PieceData.FigureLocationMap[row["FigureLocation"].ToString()] : FigureLocationEnum.None,
                    spriteID =　row["SpriteID"] != DBNull.Value　? Convert.ToInt32(row["SpriteID"].ToString()[1..]) :　-1,
                    effect = row["Effect"] != DBNull.Value ? Resources.Load<GameObject>(row["Effect"].ToString()) : null,
                    nextID = row["NextID"] != DBNull.Value ? Array.ConvertAll(row["NextID"].ToString().Split(','), int.Parse) : new[] {-1}
                };
                pieceDataList.Add(pieceData);
            }
            return pieceDataList;
        }
        
        /// <summary>
        /// 通过Excel数据生成多个ScriptableObject数据
        /// </summary>
        /// <param name="filePath">Excel文件读取路径</param>
        /// <param name="assetPath">ScriptableObject存放路径</param>
        public static void GenerateScriptableObject(string filePath, string assetPath)
        {
            var sheets = ExcelHelper.ReadExcelToPieceDataList(filePath);

            foreach (var (sheetName, dataList) in sheets)
            {
                // 生成ScriptableObject实例
                var myScriptableObject = ScriptableObject.CreateInstance<DialogueData>();
                myScriptableObject.pieceDataList = dataList;
                myScriptableObject.name = sheetName;
                
                // 保存ScriptableObject
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
            else throw new Exception("路径为空");
        }
    }
}