namespace Hoeyer.OpcUa.Core.CompileTime.CodeDomain;

public static class WellKnown
{
    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName EntityAttribute = GetTypeName("OpcUaEntityAttribute");

        public static readonly FullyQualifiedTypeName EntityBehaviourAttribute =
            GetTypeName("OpcUaEntityMethodsAttribute");

        private static FullyQualifiedTypeName GetTypeName(string className)
        {
            return new FullyQualifiedTypeName("Hoeyer.OpcUa.Core." + className);
        }
    }
}