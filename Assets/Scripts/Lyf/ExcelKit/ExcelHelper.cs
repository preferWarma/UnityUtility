using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Lyf.Utils.CodeCreate;
using UnityEngine;

namespace Lyf.ExcelKit
{
    //第一行为此描述, 说明该表的格式:
    //第二行为成员变量名,
    //第三行为成员变量类型(若为枚举类型,则格式Enum:枚举名(枚举1,枚举2)即可, 其中不允许有多余空格), 若有多个选项请使用类似int[]的格式且内容里多个选项用逗号隔开,不允许有多余空格,
    //第四行为该变量的描述(该行可以为空, 成员变量名和类型不允许为空)
    public static class ExcelHelper
    {
        /// <summary>
        /// 读取Excel表转为字典，key为sheet名，value为DataTable, 会跳过第一行数据(作为描述), 第二行为列名
        /// </summary>
        public static Dictionary<string, DataTable> ReadExcel(string excelPath)
        {
            // 创建reader对象
            using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // 设置配置项后将Excel文件转换为DataSet
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    FilterRow = rowReader => rowReader.Depth >= 1 // 跳过第一行数据(作为描述)
                }
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
        /// 将Excel表转为行数据列表的字典, key为sheet名，value为行数据列表
        /// </summary>
        /// <param name="excelPath"></param>
        /// <typeparam name="T">表示一行数据的类, 需要可以new()</typeparam>
        /// <returns></returns>
        public static Dictionary<string, List<T>> ReadExcelToRowClassList <T>(string excelPath) where T : new()
        {
            var res = new Dictionary<string, List<T>>();
            var excelData = ReadExcel(excelPath);
            foreach (var (sheetName, dataTable) in excelData)
            {
                var rowClassList = ScriptableDataGenerate.ConvertDataTableToList<T>(dataTable);
                res.Add(sheetName, rowClassList);
            }
            return res;
        }
        
        /// <summary>
        /// 根据所给的DataTable生成对应的类和枚举，其中类会生成两个， 一个为行数据类，一个为表数据类(ScriptableObject类型)
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="savePathFolder"></param>
        /// <param name="rowClassName"></param>
        /// <param name="tableClassName"></param>
        /// <param name="codeNameSpace"></param>
        /// <returns> 返回可调用的已生成的行类名和表类名(其中0号位为行类名，1号位为表类名)</returns>
        public static string[] GenerateClassAndEnumCode(DataTable dataTable, string savePathFolder, string rowClassName, string tableClassName, string codeNameSpace)
        {
            // 获取成员变量名和类型(此时的dataTable是已经跳过了第一行描述的)
            var memberNames = dataTable.Rows[0].ItemArray.Select(x => x.ToString()).ToArray();
            var memberTypes = dataTable.Rows[1].ItemArray.Select(x => x.ToString()).ToArray();
            for (var i = 0; i < memberTypes.Length; i++)
            {
                var memberType = memberTypes[i];
                // 如果是枚举类型, 需要创建枚举类
                if (memberType.StartsWith("Enum:"))
                {
                    // :和（之间的内容为枚举名
                    var enumName = memberType.Split(':')[1].Split('(')[0];
                    // 枚举的选项为（和）之间的内容，用逗号分隔
                    var enumOptions = memberType.Split('(')[1].Split(')')[0].Split(',').ToList();
                    // 创建枚举类
                    CodeGenerator.CreateEnum(enumName, enumOptions, savePathFolder, codeNameSpace);
                    // 将成员变量类型替换为枚举名
                    memberTypes[i] = codeNameSpace + "." + enumName;
                }
            }

            var members = new List<MemberStruct>();
            for (var i = 0; i < memberNames.Length; i++)
            {
                members.Add(new MemberStruct(memberTypes[i], memberNames[i]));
            }
            // 生成该Sheet每一行对应的类
            CodeGenerator.CreateClass(rowClassName, members, savePathFolder, codeNameSpace, serializable: true);
            
            // 生成表数据类：ScriptableObject类型
            members.Clear();
            members.Add(new MemberStruct($"List<{codeNameSpace}.{rowClassName}>", "dataList"));
            CodeGenerator.CreateClass(tableClassName, members, savePathFolder, codeNameSpace, nameof(ScriptableObject));
            
            // 返回可调用的行类名和表类名
            return new[] {$"{codeNameSpace}.{rowClassName}", $"{codeNameSpace}.{tableClassName}"};
        }
    }
}