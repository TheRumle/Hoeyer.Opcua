namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

public record ServiceInterfaceSourceCode(string Type, string SourceCodeString, AgentSourceCode AgentDefinition)
{
    public string AllSourceCode { get; } = AgentDefinition.SourceCodeString + "\n\n" + SourceCodeString;
    public string SourceCodeString { get; init; } = SourceCodeString;
    public AgentSourceCode AgentDefinition { get; init; } = AgentDefinition;
    public override string ToString() => Type;
}