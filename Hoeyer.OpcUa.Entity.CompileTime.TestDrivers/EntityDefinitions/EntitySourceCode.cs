namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public record EntitySourceCode(Type Type, string SourceCodeString)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Type.Name;
    }
};