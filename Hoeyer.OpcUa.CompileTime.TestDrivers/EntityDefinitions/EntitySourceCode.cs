namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public record EntitySourceCode(string Type, string SourceCodeString)
{
    public override string ToString() => Type;
}