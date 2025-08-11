using System;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

/// <summary>
///     An exception indicating that an <see cref="IAgentProvider{T}" /> has been registered for the agent but is unable
///     to get the node.
///     This exception is thrown if, for instance, an <see cref="IAgentManager" /> has not been configured for the node.
/// </summary>
public sealed class AgentProviderException(string s) : Exception(s)
{
    public AgentProviderException(Type agent) : this(
        $"An IAgentProvider has been registered for the agent '{agent.FullName}', but the provider could not get the agent.")
    {
    }

    public AgentProviderException(Type agent, string because) : this(
        $"An IAgentProvider has been registered for the agent '{agent.FullName}', but the provider could not get the agent. " +
        because)
    {
    }
}