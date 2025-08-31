namespace Hoeyer.OpcUa.Core.CompileTime.CodeDomain;

public record FullyQualifiedTypeName
{
    public FullyQualifiedTypeName(string fullyQualifiedType)
    {
        WithoutGlobalPrefix = fullyQualifiedType;
        WithGlobalPrefix = $"global::{WithoutGlobalPrefix}";
    }

    public string WithoutGlobalPrefix { get; }

    public string WithGlobalPrefix { get; }

    public bool Matches(string other)
    {
        return WithGlobalPrefix.Equals(other) || WithoutGlobalPrefix.Equals(other);
    }
}