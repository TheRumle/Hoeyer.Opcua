namespace Hoeyer.OpcUa.Core.Api;

public static class IAgentExtensions
{
    public static AgentStructure ToStructureOnly(this IAgent node) =>
        new(node);
}