using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Abstractions.NodeManagement;

public interface IEntityNodeManager<T> : IEntityNodeManager;

public interface IEntityNodeManager : INodeManager2
{
    public IManagedEntityNode ManagedEntity { get; }
}