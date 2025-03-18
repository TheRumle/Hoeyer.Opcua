namespace Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
public record FullyQualifiedTypeName
{
    public FullyQualifiedTypeName(string fullyQualifiedType)
    {
        WithoutGlobalPrefix = fullyQualifiedType;
        WithGlobalPrefix = $"global::{WithoutGlobalPrefix}";
    }

    public string WithoutGlobalPrefix { get; }

    public string WithGlobalPrefix { get; }

    public bool Matches(string other) =>WithGlobalPrefix.Equals(other) || WithoutGlobalPrefix.Equals(other);
}

public static class WellKnown
{
    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName EntityAttribute = GetTypeName("OpcUaEntityAttribute");
        private static FullyQualifiedTypeName GetTypeName(string className) => new("Hoeyer.OpcUa.Core."+className);
    }
    
}