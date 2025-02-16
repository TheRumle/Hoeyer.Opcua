using System;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
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
    
    internal EntityNodeManager Create(IServerInternal server, EntityServerConfiguration configuration,  IEntityNodeCreator nodeCreator)
    {
     
        var nodeNamespace = configuration.Host + $"/Manager/{nodeCreator.EntityName}";
        var logger = loggerFactory.CreateLogger(nodeCreator.EntityName + nameof(EntityNodeManager));
        
        Func<ushort, EntityNode> nodeCreation = namespaceIndex =>
        {
            var node = nodeCreator.CreateEntityOpcUaNode(CreateRootFolder(nodeCreator.EntityName, namespaceIndex),
                namespaceIndex);
            node.Namespace = nodeNamespace;
            return node;
        };
        
        
        return new EntityNodeManager(
            nodeNamespace,
            nodeCreation,
            server,
            logger);
    }
    
}