namespace Hoeyer.OpcUa.Compiletime.Test.Fixtures.EntityDefinitions;

public record EntitySourceCode(string Type, string SourceCodeString)
{
    public override string ToString() => Type;
}