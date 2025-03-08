using System;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    internal IEntityNodeManager Create(IServerInternal server, Uri host, IEntityNodeCreator nodeCreator)
    {
        var nodeNamespace = host.Host + $"/{nodeCreator.EntityName}";
        var logger = loggerFactory.CreateLogger(nodeCreator.EntityName + "Manager");
        var managedNode = CreatedManagedNode(server, nodeNamespace, nodeCreator);

        logger.LogInformation("Creating {@Manager} for {@ManagedNode}", nameof(EntityNodeManager), managedNode);

        return new EntityNodeManager(
            managedNode,
            server,
            new EntityHandleManager(managedNode),
            new EntityWriter(managedNode, () => server.DefaultSystemContext.Copy()),
            new EntityBrowser(managedNode),
            new EntityReader(managedNode, new PropertyReader()),
            new EntityReferenceLinker(managedNode),
            logger);
    }

    private static ManagedEntityNode CreatedManagedNode(IServerInternal server, string nodeNamespace,
        IEntityNodeCreator nodeCreator)
    {
        var namespaceIndex = server.NamespaceUris.GetIndexOrAppend(nodeNamespace);
        var context = server.DefaultSystemContext;
        var node = nodeCreator.CreateEntityOpcUaNode(namespaceIndex);
        node.Entity.Create(context, node.Entity.NodeId, node.Entity.BrowseName, node.Entity.DisplayName, false);
        return new ManagedEntityNode(node, nodeNamespace, namespaceIndex);
    }
}