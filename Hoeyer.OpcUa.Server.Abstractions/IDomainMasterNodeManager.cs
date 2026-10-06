using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IDomainMasterNodeManager : IMasterNodeManager, IDisposable
{
    IEnumerable<IEntityNodeManager> Managers { get; }
    IEnumerable<IManagedEntityNode> Nodes { get; set; }
}