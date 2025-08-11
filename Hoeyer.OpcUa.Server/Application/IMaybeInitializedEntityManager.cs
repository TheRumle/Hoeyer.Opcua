using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// A node manager that is not initialised until the OpcUa Agent server has been started, as marked by the <see cref="Hoeyer.OpcUa.Server.Api.AgentServerStartedMarker"/> being completed.
/// </summary>
public interface IMaybeInitializedAgentManager
{
    public string AgentName { get; }
    bool HasValue { get; }
    IAgentManager? Manager { get; }
}