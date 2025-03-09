using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Management;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class DomainMasterNodeManagerFactory(
    EntityNodeManagerFactory entityManagerFactory,
    IEnumerable<IEntityNodeCreator> creators,
    IOpcUaEntityServerInfo info
) : IDomainMasterManagerFactory
{
    /// <inheritdoc />
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server,
        ApplicationConfiguration applicationConfiguration)
    {
        var additionalManagers = creators.Select(e => entityManagerFactory.Create(server, info.Host, e)).ToArray();
        return new DomainMasterNodeManager(server, applicationConfiguration, additionalManagers);
    }
}