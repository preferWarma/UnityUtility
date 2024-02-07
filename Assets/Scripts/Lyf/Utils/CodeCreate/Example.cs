using System.Collections.Generic;
using UnityEditor;

namespace Lyf.Utils.CodeCreate
{
    public class Example
    {
        [MenuItem("Tools/CodeCreate/TestGenerateClass")]
        public static void TestGenerateClass()
        {
            // 生成代码
            var members = new List<MemberStruct>
            {
                new("int", "TestInt"),
                new("string", "TestString"),
                new("float", "TestFloat"),
                new ("int[]", "TestIntArray"),
                new ($"{typeof(Example).Namespace}.TestEnum", "TestEnum"),
            };
            CodeGenerator.CreateClass("TestClass", members, "Assets/Scripts/Generation",typeof(Example).Namespace, "MonoBehaviour");
        }
        
        // 生成枚举代码
        [MenuItem("Tools/CodeCreate/TestGenerateEnum")]
        public static void TestGenerateEnum()
        {
            // 生成代码
            var members = new List<string>
            {
                "Test1",
                "Test2",
                "Test3"
            };
            CodeGenerator.CreateEnum("TestEnum", members, "Assets/Scripts/Generation", typeof(Example).Namespace);
        }
    }
}