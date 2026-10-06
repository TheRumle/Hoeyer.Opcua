using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class DomainMasterNodeManager : MasterNodeManager, IDomainMasterNodeManager
{
    /// <inheritdoc />
    public DomainMasterNodeManager(IServerInternal server, ApplicationConfiguration applicationConfiguration,
        IEntityNodeManager[] additionalManagers) : base(server, applicationConfiguration,
        applicationConfiguration.ApplicationUri, additionalManagers)
    {
        Nodes = additionalManagers.Select(e => e.ManagedEntity);
    }

    public IEnumerable<IEntityNodeManager> Managers => this.NodeManagers.OfType<IEntityNodeManager>();

    public IEnumerable<IManagedEntityNode> Nodes { get; set; }
}