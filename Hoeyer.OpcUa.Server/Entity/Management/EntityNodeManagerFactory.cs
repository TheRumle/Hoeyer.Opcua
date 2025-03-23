using System;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    internal IEntityNodeManager Create(ManagedEntityNode managedNode, IServerInternal server)
    {
        var entityName = managedNode.BaseObject.DisplayName.Text;
        var logger = loggerFactory.CreateLogger(entityName + "Manager");

        logger.LogInformation("Creating {@Manager} for {@ManagedNode}", nameof(EntityNodeManager), managedNode);

        return new EntityNodeManager(
            managedNode,
            server,
            new EntityHandleManager(managedNode),
            new EntityWriter(managedNode),
            new EntityBrowser(managedNode),
            new EntityReader(managedNode, new PropertyReader()),
            new EntityReferenceLinker(managedNode),
            logger);
    }


}