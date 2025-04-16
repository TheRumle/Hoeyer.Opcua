using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityNodeManager : INodeManager2
{
    public IEntityNode ManagedEntity { get; }
}