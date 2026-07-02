using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class DomainMasterNodeManager : MasterNodeManager
{
    /// <inheritdoc />
    public DomainMasterNodeManager(IServerInternal server, ApplicationConfiguration applicationConfiguration,
        IEntityNodeManager[] additionalManagers) : base(server, applicationConfiguration,
        applicationConfiguration.ApplicationUri, additionalManagers)
    {
        Nodes = additionalManagers.Select(e => e.ManagedEntity);
    }

    public IEnumerable<IManagedEntityNode> Nodes { get; set; }

    public IEnumerable<IEntityNode> ManagedEntities => Nodes.Select(e => e.Select(node => node));
}