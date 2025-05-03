using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

internal interface IEntityNodeManager<T> : IEntityNodeManager;

internal interface IEntityNodeManager : INodeManager2
{
    public IManagedEntityNode ManagedEntity { get; }
}