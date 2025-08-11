namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

/// <summary>
/// Represent source code for both an Agent and an accompagnying service that depends on the source code definition of the Agent. 
/// </summary>
/// <param name="ServiceName">The name of the interface</param>
/// <param name="AgentName">The name of the agent</param>
/// <param name="CombinedSourceCode"></param>
public sealed record AgentAndServiceSourceCode(
    string ServiceName,
    string AgentName,
    string AgentSourceCode,
    string ServiceSourceCode)
{
    /// <inheritdoc />
    public override string ToString() => ServiceName + " and " + AgentName;
}