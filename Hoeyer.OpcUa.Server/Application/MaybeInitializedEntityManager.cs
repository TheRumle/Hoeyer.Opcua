using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaAgentService(typeof(MaybeInitializedAgentManager<>), ServiceLifetime.Singleton)]
public sealed class MaybeInitializedAgentManager<T> : IMaybeInitializedAgentManager
{
    /// <inheritdoc />
    public string AgentName { get; } = typeof(T).Name;

    public bool HasValue => Manager != null;
    public IAgentManager? Manager { get; internal set; }
}