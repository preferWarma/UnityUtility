using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lyf.Dialogue
{
    [Serializable]
    public class PieceData
    {
        [Tooltip("单句ID")] public int id;
        [Tooltip("单句类型")] public TypeEnum type;
        [Tooltip("句子具体内容")] public string detail;
        [Tooltip("人物ID(句子为选项的时候为空,记为-1)")] public int figureID;
        [Tooltip("人物显示位置(句子为选项的时候为空,记为None)")] public FigureLocationEnum figureLocation;
        [Tooltip("图片位置(句子为选项的时候为空,记为-1)")] public int spriteID;
        [Tooltip("句子呈现效果(没有可记为null)")] public GameObject effect;
        [Tooltip("下一条ID(当为最后一条对话时为空, 记为-1)")] public int[] nextID;
        
        // 枚举类的映射表
        public static Dictionary<int, TypeEnum> IDMap = new() {{1, TypeEnum.Single}, {2, TypeEnum.SingleWithOption}, {3, TypeEnum.Option}};
        public static Dictionary<string, FigureLocationEnum> FigureLocationMap = new()
        {
            {"None", FigureLocationEnum.None},
            {"L", FigureLocationEnum.L},
            {"M", FigureLocationEnum.M},
            {"R", FigureLocationEnum.R}
        };
    }// class PieceData
    
    public enum TypeEnum    // 单句类型
    {
        Single, // 普通单句
        SingleWithOption, // 带选项的单句
        Option // 选项
    }
    
    public enum FigureLocationEnum  // 人物显示位置
    {
        None,   // 无
        L,  // 左
        M,  // 中
        R   // 右
    }

    
    
}// namespace Automate