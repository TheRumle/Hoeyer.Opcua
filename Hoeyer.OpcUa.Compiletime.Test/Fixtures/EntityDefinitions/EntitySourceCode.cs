namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;

public record EntitySourceCode(string Type, string SourceCodeString)
{
    public override string ToString() => Type;
}