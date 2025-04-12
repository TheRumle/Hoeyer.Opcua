namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public record EntitySourceCode(string Type, string SourceCodeString)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Type;
    }
}