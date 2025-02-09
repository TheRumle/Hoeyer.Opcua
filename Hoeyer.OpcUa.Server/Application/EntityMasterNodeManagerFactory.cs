using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class EntityMasterNodeManagerFactory(IEnumerable<IEntityNodeCreator> nodeCreators)
{
    public EntityMasterNodeManager Create(IServerInternal server, ApplicationConfiguration serverApplicationConfiguration, EntityServerConfiguration details)
    {
        return new EntityMasterNodeManager(server, serverApplicationConfiguration, nodeCreators
            .Select(e => new EntityNodeManager(e, server, serverApplicationConfiguration, details)).ToArray());
    }
}