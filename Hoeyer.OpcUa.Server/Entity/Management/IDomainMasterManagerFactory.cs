using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

public interface IDomainMasterManagerFactory
{
    /// <summary>
    /// </summary>
    /// <param name="server"></param>
    /// <param name="applicationConfiguration"></param>
    /// <returns></returns>
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server,
        ApplicationConfiguration applicationConfiguration);
}