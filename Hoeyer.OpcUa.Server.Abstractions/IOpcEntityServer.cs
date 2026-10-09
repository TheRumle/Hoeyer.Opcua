using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IOpcEntityServer : IStandardServer
{
    IEnumerable<IEntityManagerHolder> Managers { get; }
    IDomainMasterNodeManager? DomainManager { get; }
    public ServerBase AsServerBase();
    public ISystemContext DefaultContext { get; }
}