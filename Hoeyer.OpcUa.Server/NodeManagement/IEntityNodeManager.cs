using Hoeyer.OpcUa.Core.Entity;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.NodeManagement;

public interface IEntityNodeManager : INodeManager2
{
    public IEntityNode ManagedEntity { get; }
}