using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class DomainMasterNodeManager : MasterNodeManager
{
    /// <inheritdoc />
    public DomainMasterNodeManager(IServerInternal server, ApplicationConfiguration applicationConfiguration,
        IAgentManager[] additionalManagers) : base(server, applicationConfiguration,
        applicationConfiguration.ApplicationUri, additionalManagers)
    {
        Nodes = additionalManagers.Select(e => e.ManagedAgent);
    }

    public IEnumerable<IManagedAgent> Nodes { get; set; }

    public IEnumerable<IAgent> ManagedEntities => Nodes.Select(e => e.Select(node => node));
}