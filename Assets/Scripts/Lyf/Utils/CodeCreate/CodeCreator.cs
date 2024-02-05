// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Microsoft.CodeAnalysis.CSharp;
//
// namespace Lyf.Utils.CodeCreate
// {
//     public class CodeCreator
//     {
//         // // <summary>
//         // /// 生成类
//         // /// </summary>
//         // /// <param name="className">类名</param>
//         // /// <param name="members">类成员</param>
//         // /// <param name="savePathFolder">文件保存路径(不包括类名)</param>
//         // /// <param name="baseClassName">基类</param>
//         // /// <param name="classNamespace">类命名空间</param>
//         // private static void CreateClass(string className, List<MemberStruct> members, string savePathFolder = "./Generation", 
//         //     string baseClassName = null, string classNamespace = "DefaultNameSpace")
//         // {
//         //     // 生成代码
//         //     var compileUnit = new CodeCompileUnit();
//         //
//         //     // 命名空间
//         //     var codeNamespace = new CodeNamespace(classNamespace);
//         //     compileUnit.Namespaces.Add(codeNamespace);
//         //
//         //     // 类型声明
//         //     var classDeclaration = new CodeTypeDeclaration(className)
//         //     {
//         //         IsClass = true
//         //     };
//         //
//         //     // 继承的基类
//         //     if (!string.IsNullOrEmpty(baseClassName))
//         //     {
//         //         classDeclaration.BaseTypes.Add(new CodeTypeReference(baseClassName));
//         //     }
//         //     codeNamespace.Types.Add(classDeclaration);
//         //
//         //     // 成员声明
//         //     foreach (var member in members)
//         //     {
//         //         var memberField = new CodeMemberField(member.MemberType, member.MemberName)
//         //         {
//         //             Attributes = member.IsPublic ? MemberAttributes.Public : MemberAttributes.Private
//         //         };
//         //         classDeclaration.Members.Add(memberField);
//         //     }
//         //     
//         //     // 将生成的代码保存到文件
//         //     var savePath = savePathFolder + (savePathFolder.EndsWith("/") ? "" : "/") + className + ".cs";
//         //     using (var sw = new StreamWriter(savePath))
//         //     {
//         //         var codeProvider = new CSharpCodeProvider();
//         //         codeProvider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
//         //     }
//         //
//         //     Console.WriteLine($"代码已生成到文件: {savePath}");
//         // }
//         //
//         //
//         //
//         // /// <summary>
//         // /// 生成枚举
//         // /// </summary>
//         // /// <param name="enumName">枚举类名</param>
//         // /// <param name="enumMembers">枚举成员</param>
//         // /// <param name="savePathFolder">文件保存路径(不包括类名)</param>
//         // /// <param name="enumNamespace">枚举类的命名空间</param>
//         // private static void CreateEnum(string enumName, List<string> enumMembers, string savePathFolder = "./Generation",
//         //     string enumNamespace = "DefaultNameSpace")
//         // {
//         //     // 生成代码
//         //     var compileUnit = new CodeCompileUnit();
//         //
//         //     // 命名空间
//         //     var codeNamespace = new CodeNamespace(enumNamespace);
//         //     compileUnit.Namespaces.Add(codeNamespace);
//         //
//         //     // 枚举声明
//         //     var enumDeclaration = new CodeTypeDeclaration(enumName)
//         //     {
//         //         IsEnum = true
//         //     };
//         //     codeNamespace.Types.Add(enumDeclaration);
//         //
//         //     // 枚举成员
//         //     foreach (var member in enumMembers)
//         //     {
//         //         enumDeclaration.Members.Add(new CodeMemberField(enumName, member));
//         //     }
//         //     
//         //     // 将生成的代码保存到文件
//         //     if (!Directory.Exists(savePathFolder))
//         //     {
//         //         Directory.CreateDirectory(savePathFolder);
//         //     }
//         //     var savePath = savePathFolder + (savePathFolder.EndsWith("/") ? "" : "/") + enumName + ".cs";
//         //     using (var sw = new StreamWriter(savePath))
//         //     {
//         //         var codeProvider = new CSharpCodeProvider();
//         //         codeProvider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions());
//         //     }
//         //
//         //     Console.WriteLine($"代码已生成到文件: {savePath}");
//         // }
//         
//         private static void CreateClass(string className, List<MemberStruct> members, string savePathFolder = "./Generation",
//                                     string baseClassName = null, string classNamespace = "DefaultNamespace")
//     {
//         // 生成代码
//         var compilationUnit = SyntaxFactory.CompilationUnit()
//             .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
//             .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")))
//             .AddMembers(
//                 SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(classNamespace))
//                     .AddMembers(
//                         SyntaxFactory.ClassDeclaration(className)
//                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
//                             .WithBaseList(baseClassName != null
//                                 ? SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
//                                     SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClassName))))
//                                 : null)
//                             .AddMembers(members.Select(member => GenerateMemberDeclaration(member)).ToArray())
//                     )
//             );
//
//         // 将生成的代码保存到文件
//         string filePath = Path.Combine(savePathFolder, $"{className}.cs");
//         using (StreamWriter sw = new StreamWriter(filePath))
//         {
//             SyntaxTree syntaxTree = SyntaxFactory.SyntaxTree(compilationUnit);
//             syntaxTree.GetRoot().NormalizeWhitespace().WriteTo(sw);
//         }
//
//         Console.WriteLine($"代码已生成到文件: {filePath}");
//     }
//
//     private static void CreateEnum(string enumName, List<string> enumMembers, string savePathFolder = "./Generation",
//                                    string enumNamespace = "DefaultNamespace")
//     {
//         // 生成代码
//         var compilationUnit = SyntaxFactory.CompilationUnit()
//             .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
//             .AddMembers(
//                 SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(enumNamespace))
//                     .AddMembers(
//                         SyntaxFactory.EnumDeclaration(enumName)
//                             .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
//                             .AddMembers(enumMembers.Select(member => SyntaxFactory.EnumMemberDeclaration(member)).ToArray())
//                     )
//             );
//
//         // 将生成的代码保存到文件
//         if (!Directory.Exists(savePathFolder))
//         {
//             Directory.CreateDirectory(savePathFolder);
//         }
//         string filePath = Path.Combine(savePathFolder, $"{enumName}.cs");
//         using (StreamWriter sw = new StreamWriter(filePath))
//         {
//             SyntaxTree syntaxTree = SyntaxFactory.SyntaxTree(compilationUnit);
//             syntaxTree.GetRoot().NormalizeWhitespace().WriteTo(sw);
//         }
//
//         Console.WriteLine($"代码已生成到文件: {filePath}");
//     }
//
//     private static MemberDeclarationSyntax GenerateMemberDeclaration(MemberStruct member)
//     {
//         // 生成成员声明
//         return SyntaxFactory.FieldDeclaration(
//                 SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(member.MemberType))
//                     .AddVariables(SyntaxFactory.VariableDeclarator(member.MemberName)))
//             .AddModifiers(member.IsPublic
//                 ? SyntaxFactory.Token(SyntaxKind.PublicKeyword)
//                 : SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
//     }
//     }
// }