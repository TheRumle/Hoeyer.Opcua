using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

internal interface IAgentManager<T> : IAgentManager;

public interface IAgentManager : INodeManager2
{
    public IManagedAgent ManagedEntity { get; }
}