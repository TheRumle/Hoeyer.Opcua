using System;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;

public sealed class EntityNodeManager : CustomNodeManager
{
    private readonly IEntityNodeCreator _iEntityObjectCreator;
    public readonly FolderState Folder ;
    public NodeState EntityState { get; }
    public readonly EntityNode EntityNode;


    public EntityNodeManager(IEntityNodeCreator entityNodeCreator, IServerInternal server, ApplicationConfiguration configuration,
        EntityServerConfiguration entityServerConfiguration): base(server, configuration, entityServerConfiguration.Urn.ToString())
    {
        _iEntityObjectCreator = entityNodeCreator;
        Folder = CreateRootFolder();
        EntityState = _iEntityObjectCreator.CreateEntityOpcUaNode(Folder,0);
        EntityNode = new EntityNode(Folder, EntityState);
    }



    /// <inheritdoc />
    protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
    {
        return new NodeStateCollection([EntityState]);
    }

    /// <inheritdoc />
    protected override void AddPredefinedNode(ISystemContext context, NodeState node)
    {
        Console.WriteLine(node.BrowseName);
        base.AddPredefinedNode(context, node);
    }


    
    private FolderState CreateRootFolder()
    {
        // Create a root folder for this entity.
        var rootFolder = new FolderState(null)
        {
            BrowseName = new QualifiedName(_iEntityObjectCreator.EntityName, NamespaceIndex),
            DisplayName = _iEntityObjectCreator.EntityName,
            NodeId = new NodeId(_iEntityObjectCreator.EntityName, NamespaceIndex),
            TypeDefinitionId = ObjectTypeIds.NamespacesType,
        };

        return rootFolder;
    }
}