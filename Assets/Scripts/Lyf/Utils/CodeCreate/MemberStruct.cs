namespace Lyf.Utils.CodeCreate
{
    public struct MemberStruct
    {
        public readonly string MemberType;
        public readonly string MemberName;
        public readonly bool IsPublic;

        public MemberStruct(string memberType, string memberName, bool isPublic = true)
        {
            MemberName = memberName;
            MemberType = memberType;
            IsPublic = isPublic;
        }
    }
}