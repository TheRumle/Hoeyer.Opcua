namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public record ServiceInterfaceSourceCode(string Type, string SourceCodeString, EntitySourceCode EntityDefinition)
{
    public string AllSourceCode { get; } = EntityDefinition.SourceCodeString + "\n\n" + SourceCodeString;
    public string SourceCodeString { get; init; } = SourceCodeString;
    public EntitySourceCode EntityDefinition { get; init; } = EntityDefinition;
    public override string ToString() => Type;
}