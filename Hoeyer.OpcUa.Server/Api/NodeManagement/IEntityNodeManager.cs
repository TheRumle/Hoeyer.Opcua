using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

internal interface IEntityNodeManager<T> : IEntityNodeManager;

public interface IEntityNodeManager : INodeManager2
{
    public IManagedEntityNode ManagedEntity { get; }
}