using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using UnityEditor;
using UnityEngine;

namespace Lyf.Utils.CodeCreate
{
    public static class CodeGenerator
    {
        /// <summary>
        /// 关键字映射表
        /// </summary>
        private static readonly Dictionary<string, string> KeyWords = new()
        {
            {"int", "System.Int32"},
            {"float", "System.Single"},
            {"double", "System.Double"},
            {"string", "System.String"},
            {"bool", "System.Boolean"},
            {"long", "System.Int64"},
            {"short", "System.Int16"},
            {"byte", "System.Byte"},
            {"char", "System.Char"},
            {"decimal", "System.Decimal"},
            {"object", "System.Object"},
            {"sbyte", "System.SByte"},
            {"uint", "System.UInt32"},
            {"ulong", "System.UInt64"},
            {"ushort", "System.UInt16"},
            {"void", "System.Void"},
            {"int[]", "System.Int32[]"},
            {"float[]", "System.Single[]"},
            {"double[]", "System.Double[]"},
            {"string[]", "System.String[]"},
            {"bool[]", "System.Boolean[]"},
            {"long[]", "System.Int64[]"},
            {"short[]", "System.Int16[]"},
            {"byte[]", "System.Byte[]"},
            {"char[]", "System.Char[]"},
            {"decimal[]", "System.Decimal[]"},
            {"object[]", "System.Object[]"},
            {"sbyte[]", "System.SByte[]"},
            {"uint[]", "System.UInt32[]"},
            {"ulong[]", "System.UInt64[]"},
            {"ushort[]", "System.UInt16[]"},
            {"void[]", "System.Void[]"}
        };
        
        /// <summary>
        /// 生成类
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="members">类成员</param>
        /// <param name="savePathFolder">文件保存路径(不包括类名)</param>
        /// <param name="baseClassName">基类</param>
        /// <param name="classNamespace">类命名空间</param>
        /// <param name="reference">using引用目录(默认含有System和UnityEngine)</param>
        public static void CreateClass(string className, List<MemberStruct> members, string savePathFolder = "Assets/Scripts/Generation", 
            string baseClassName = null, string classNamespace = "DefaultNameSpace", List<string> reference = null)
        {
            // 生成代码
            var compileUnit = new CodeCompileUnit();

            // 命名空间
            var codeNamespace = new CodeNamespace(classNamespace);
            // 添加using
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            if (reference != null)
            {
                foreach (var usingName in reference)
                {
                    codeNamespace.Imports.Add(new CodeNamespaceImport(usingName));
                }
            }
            // 添加到编译单元
            compileUnit.Namespaces.Add(codeNamespace);

            // 类型声明
            var classDeclaration = new CodeTypeDeclaration(className)
            {
                IsClass = true
            };

            // 继承的基类
            if (!string.IsNullOrEmpty(baseClassName))
            {
                classDeclaration.BaseTypes.Add(new CodeTypeReference(baseClassName));
            }
            codeNamespace.Types.Add(classDeclaration);

            // 成员声明
            foreach (var member in members)
            {
                if (!KeyWords.ContainsKey(member.MemberType))
                {
                    Debug.LogError($"不支持的类型: {member.MemberType}");
                    continue;
                }
                var memberType = KeyWords[member.MemberType];
                var memberField = new CodeMemberField(memberType, member.MemberName)
                {
                    Attributes = member.IsPublic ? MemberAttributes.Public : MemberAttributes.Private
                };
                classDeclaration.Members.Add(memberField);
            }
            
            // 将生成的代码保存到文件
            if (!Directory.Exists(savePathFolder))
            {
                Directory.CreateDirectory(savePathFolder);
            }
            
            // 将生成的代码保存到文件
            var savePath = savePathFolder + (savePathFolder.EndsWith("/") ? "" : "/") + className + ".cs";
            using (var sw = new StreamWriter(savePath))
            {
                var codeProvider = new CSharpCodeProvider();
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
            }
            AssetDatabase.Refresh();
            Debug.Log($"{className}.cs已生成到文件: {savePath}");
        }

        
        
        /// <summary>
        /// 生成枚举
        /// </summary>
        /// <param name="enumName">枚举类名</param>
        /// <param name="enumMembers">枚举成员</param>
        /// <param name="savePathFolder">文件保存路径(不包括类名)</param>
        /// <param name="enumNamespace">枚举类的命名空间</param>
        public static void CreateEnum(string enumName, List<string> enumMembers, string savePathFolder = "Assets/Scripts/Generation",
            string enumNamespace = "DefaultNameSpace")
        {
            // 生成代码
            var compileUnit = new CodeCompileUnit();

            // 命名空间
            var codeNamespace = new CodeNamespace(enumNamespace);
            compileUnit.Namespaces.Add(codeNamespace);

            // 枚举声明
            var enumDeclaration = new CodeTypeDeclaration(enumName)
            {
                IsEnum = true
            };
            codeNamespace.Types.Add(enumDeclaration);

            // 枚举成员
            foreach (var member in enumMembers)
            {
                enumDeclaration.Members.Add(new CodeMemberField(enumName, member));
            }
            
            // 将生成的代码保存到文件
            if (!Directory.Exists(savePathFolder))
            {
                Directory.CreateDirectory(savePathFolder);
            }
            var savePath = savePathFolder + (savePathFolder.EndsWith("/") ? "" : "/") + enumName + ".cs";
            using (var sw = new StreamWriter(savePath))
            {
                var codeProvider = new CSharpCodeProvider();
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
            }

            AssetDatabase.Refresh();
            Debug.Log($"{enumName}.cs已生成到文件: {savePath}");
        }
    }
}