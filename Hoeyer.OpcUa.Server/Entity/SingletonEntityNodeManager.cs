﻿using System;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity;

public sealed class SingletonEntityNodeManager : CustomNodeManager{
    private readonly IEntityObjectStateCreator _entityObjectCreator;
    private readonly string _namespaceUri;
    private readonly ushort _namespaceIndex;
    public NodeState EntityNode { get; private set; } = null!;
    
    public SingletonEntityNodeManager(IEntityObjectStateCreator entityObjectCreator, 
        IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration)
    {
        _namespaceUri =  Server.NamespaceUris.ToArray()[0].EndsWith("/")
            ? server.NamespaceUris.ToArray()[0] +  entityObjectCreator.EntityName
            : server.NamespaceUris.ToArray()[0] + "/" + entityObjectCreator.EntityName;
        
        _entityObjectCreator = entityObjectCreator;
        _namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(_namespaceUri);
    }
    

    /// <inheritdoc />
    protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
    {
        var root = CreateRootFolder(context);
        EntityNode = _entityObjectCreator.CreateEntityOpcUaNode(root, _namespaceIndex);
        return new NodeStateCollection([EntityNode]);
    }

    /// <inheritdoc />
    protected override void AddPredefinedNode(ISystemContext context, NodeState node)
    {
        
        base.AddPredefinedNode(context, node);
    }


    private NodeState CreateRootFolder(ISystemContext context)
    {
        var browseName = new QualifiedName(_entityObjectCreator.EntityName+"Content", _namespaceIndex);
        FolderState root = new FolderState(null); // Parent is null for the root
        root.NodeId = new NodeId(Guid.NewGuid(), _namespaceIndex); //Give it a node id
        root.BrowseName = browseName;
        root.Description = new LocalizedText($"The root folder for my data about the {_entityObjectCreator.EntityName} entity.");
        root.ReferenceTypeId = ReferenceTypeIds.Organizes;
        root.TypeDefinitionId = ObjectTypeIds.FolderType;

        return root;
    }
}