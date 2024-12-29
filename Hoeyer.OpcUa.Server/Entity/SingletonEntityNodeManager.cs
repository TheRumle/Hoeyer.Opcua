using System;
using System.Collections.Generic;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity;

internal sealed class SingletonEntityNodeManager : CustomNodeManager{
    private readonly IEntityObjectStateCreator _entityObjectCreator;

    public SingletonEntityNodeManager(IEntityObjectStateCreator entityObjectCreator, 
        IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration)
    {
        NamespaceUris = server.NamespaceUris.ToArray();
        _entityObjectCreator = entityObjectCreator;
    }


    /// <inheritdoc />
    protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
    {
        var root = CreateRootFolder();
        BaseObjectState entityNode = _entityObjectCreator.CreateEntityOpcUaNode(context, root, NamespaceIndex);
        return new NodeStateCollection([entityNode]);
    }

    private FolderState CreateRootFolder()
    {
        FolderState root = new FolderState(null); // Parent is null for the root
        root.NodeId = new NodeId(Guid.NewGuid(), NamespaceIndex); //Give it a node id
        root.BrowseName = new QualifiedName(_entityObjectCreator.EntityName, NamespaceIndex);
        root.Description = new LocalizedText($"The root folder for my data about the {_entityObjectCreator.EntityName} entity.");
        root.ReferenceTypeId = ReferenceTypeIds.Organizes;
        root.TypeDefinitionId = ObjectTypeIds.FolderType;
        return root;
    }
}