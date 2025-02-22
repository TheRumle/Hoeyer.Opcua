using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityNodeManager : INodeManager2
{
    public ManagedEntityNode ManagedEntity { get;  }
}