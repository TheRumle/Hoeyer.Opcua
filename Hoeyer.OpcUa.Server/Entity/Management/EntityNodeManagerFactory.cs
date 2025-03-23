using System;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity.Management;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    internal IEntityNodeManager Create(IServerInternal server, Uri host, IEntityNodeFactory factory)
    {
        var nodeNamespace = host.Host + $"/{factory.EntityName}";
        var logger = loggerFactory.CreateLogger(factory.EntityName + "Manager");
        var managedNode = CreatedManagedNode(server, nodeNamespace, factory.Create);

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

    private static ManagedEntityNode CreatedManagedNode(IServerInternal server, string nodeNamespace,
        Func<ushort, IEntityNode> nodeCreator)
    {
        var namespaceIndex = server.NamespaceUris.GetIndexOrAppend(nodeNamespace);
        var context = server.DefaultSystemContext;
        var node = nodeCreator.Invoke(namespaceIndex);
        node.BaseObject.Create(context, node.BaseObject.NodeId, node.BaseObject.BrowseName, node.BaseObject.DisplayName,
            false);
        return new ManagedEntityNode(node, nodeNamespace, namespaceIndex);
    }
}