namespace Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;

public static class WellKnown
{
    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName EntityAttribute = GetTypeName("OpcUaEntityAttribute");

        private static FullyQualifiedTypeName GetTypeName(string className)
        {
            return new FullyQualifiedTypeName("Hoeyer.OpcUa.Core." + className);
        }
    }
}