using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.EntityNode.Operations;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public sealed class EntityNodeManagerFactory(ILoggerFactory loggerFactory)
{
    

    
    internal IEntityNodeManager Create(IServerInternal server, IOpcUaEntityServerInfo info,  IEntityNodeCreator nodeCreator)
    {
        var nodeNamespace = info.Host + $"Manager/{nodeCreator.EntityName}";
        var logger = loggerFactory.CreateLogger(nodeCreator.EntityName+"Manager");
        var managedNode = CreatedManagedNode(server, nodeNamespace, nodeCreator);

        logger.LogInformation("Creating {@Manager} for {@ManagedNode}", nameof(EntityNodeManager), managedNode);
        return new EntityNodeManager(
            managedNode,
            server,
            new EntityHandleManager(managedNode),
            new EntityModifier(managedNode),
            new EntityBrowser(managedNode),
            new EntityReader(managedNode),
            new EntityReferenceManager(managedNode),
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
    
    private static FolderState CreateRootFolder(string folderName, ushort namespaceIndex)
    {
        // Create a root folder for this entity.
        var rootFolder = new FolderState(null)
        {
            SymbolicName = folderName,
            BrowseName = new QualifiedName(folderName),
            DisplayName = folderName,
            NodeId = new NodeId(folderName, namespaceIndex),
            TypeDefinitionId = ObjectTypeIds.FolderType,
        };

        return rootFolder;
    }
    
}