using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
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
        ManagedEntities = additionalManagers.Select(e => e.ManagedEntity);
    }

    public IEnumerable<IEntityNode> ManagedEntities { get; private set; }
}