using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    
    private static FolderState CreateRootFolder(string folderName, ushort namespaceIndex)
    {
        // Create a root folder for this entity.
        var rootFolder = new FolderState(null)
        {
            BrowseName = new QualifiedName(folderName),
            DisplayName = folderName,
            NodeId = new NodeId(folderName, namespaceIndex),
            TypeDefinitionId = ObjectTypeIds.FolderType,
        };

        return rootFolder;
    }
    
    internal EntityNodeManager Create(IServerInternal server, OpcUaEntityServerConfiguration configuration,  IEntityNodeCreator nodeCreator)
    {
     
        var nodeNamespace = configuration.Host + $"/Manager/{nodeCreator.EntityName}";
        var logger = loggerFactory.CreateLogger(nodeCreator.EntityName + nameof(EntityNodeManager));
        ManagedEntityNode managedNode = CreatedManagedNode(server, nodeNamespace, nodeCreator);

        var handleManager = new EntityHandleManager(managedNode, logger);
        logger.LogInformation("Creating {@Manager} for {@ManagedNode}", nameof(EntityNodeManager), managedNode);
        return new EntityNodeManager(
            managedNode,
            server,
            handleManager,
            new EntityModifier(managedNode, handleManager),
            new EntityBrowser(managedNode, logger),
            logger);
    }

    private static ManagedEntityNode CreatedManagedNode(IServerInternal server, string nodeNamespace, IEntityNodeCreator nodeCreator)
    {
        var namespaceIndex = server.NamespaceUris.GetIndexOrAppend(nodeNamespace);
        var context = server.DefaultSystemContext;
        var root = CreateRootFolder(nodeCreator.EntityName, namespaceIndex);
        var node = nodeCreator.CreateEntityOpcUaNode(root, namespaceIndex);
        node.Folder.Create(context, node.Folder.NodeId, node.Folder.BrowseName, node.Folder.DisplayName, false);
        node.Entity.Create(context, node.Entity.NodeId, node.Entity.BrowseName, node.Entity.DisplayName, false);
        node.Folder.AddChild(node.Entity);
        return new ManagedEntityNode(node, nodeNamespace, namespaceIndex);
    }
    
}