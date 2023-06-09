using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Lyf.Dialogue;
using UnityEngine;

namespace Lyf.Automate
{
    public static class ExcelReader
    {
        //读取Excel文件
        private static DataTable LoadExcel(string filePath, int idx)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            // 将Excel文件转换为DataTable
            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });
            return result.Tables[idx];
        }
        
        /// <summary>
        /// 读取Excel文件, 以DataTable列表的形式返回所有的sheet的数据, 并以out 字符串列表保存所有的sheet名字
        /// </summary>
        /// <param name="filePath"> Excel 文件路径</param>
        /// <returns></returns>
        public static IEnumerable<DataTable> LoadExcel(string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            // 将Excel文件转换为DataTable
            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });
            var res = new DataTable[result.Tables.Count];
            for (var i = 0; i < result.Tables.Count; i++)
            {
                res[i] = result.Tables[i];
            }
            return res;
        }

        // 将DataTable转换为List<PieceData>
        private static List<PieceData> ConvertToPieceDataList(DataTable dataTable)
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
        /// 读取Excel文件，返回PieceData列表
        /// </summary>
        /// <param name="filePath"> Excel表文件路径</param>
        /// <param name="tableIdx"> table索引</param>
        /// <param name="sheetName"> 当前sheet的名字</param>
        /// <returns></returns>
        public static List<PieceData> LoadPieceDataSheet(string filePath, int tableIdx, out string sheetName)
        {
            var dataTable = LoadExcel(filePath, tableIdx);
            sheetName = dataTable.TableName;
            return ConvertToPieceDataList(dataTable);
        }

        /// <summary>
        /// 读取Excel文件，返回PieceData的嵌套表结构
        /// </summary>
        /// <param name="filePath"> Excel文件路径</param>
        /// <param name="sheetNames"> Excel工作表名字</param>
        /// <returns></returns>
        public static List<List<PieceData>> LoadPieceDataSheets(string filePath, out string[] sheetNames)
        {
            var dataTables = LoadExcel(filePath).ToArray();
            var names = new string[dataTables.Length];
            for (var i = 0; i < dataTables.Length; i++)
            {
                names[i] = dataTables.ElementAt(i).TableName;
            }
            sheetNames = names;
            return dataTables.Select(ConvertToPieceDataList).ToList();
        }

    }// class ExcelReader
}