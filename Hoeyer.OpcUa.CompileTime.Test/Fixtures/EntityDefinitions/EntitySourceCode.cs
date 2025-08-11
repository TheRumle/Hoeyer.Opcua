namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

public record AgentSourceCode(string Type, string SourceCodeString)
{
    public override string ToString() => Type;
}