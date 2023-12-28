using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using Lyf.Dialogue;

namespace Lyf.Automate
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 读取Excel表转为字典，key为sheet名，value为DataTable
        /// </summary>
        private static Dictionary<string, DataTable> ReadExcel(string excelPath)
        {
            // 创建reader对象
            using var stream =  File.Open(excelPath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            
            // 设置配置项后将Excel文件转换为DataSet
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });
            
            // 将DataSet转换为字典
            var res = new Dictionary<string, DataTable>();
            foreach (DataTable table in dataSet.Tables)
            {
                res.Add(table.TableName, table);
            }
            return res;
        }
        
        
        /// <summary>
        /// 将Excel表转为对话数据字典, key为sheet名，value为对话数据列表
        /// </summary>
        public static Dictionary<string, List<PieceData>> ReadExcelToPieceDataList(string excelPath)
        {
            var res = new Dictionary<string, List<PieceData>>();
            var excelData = ReadExcel(excelPath);
            foreach (var (sheetName, dataTable) in excelData)
            {
                var pieceDataList = DialogueDataGenerate.ConvertToPieceDataList(dataTable);
                res.Add(sheetName, pieceDataList);
            }
            return res;
        }
    }
    
}