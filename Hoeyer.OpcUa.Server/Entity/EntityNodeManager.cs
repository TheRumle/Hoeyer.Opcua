﻿using System;
using Hoeyer.OpcUa.Nodes;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Entity;

public sealed class EntityNodeManager : CustomNodeManager
{
    private readonly IEntityNodeCreator _iEntityObjectCreator;
    private readonly ushort _namespaceIndex;
    private readonly string _namespaceUri;

    public EntityNodeManager(IEntityNodeCreator iEntityObjectCreator,
        IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration)
    {
        _namespaceUri = Server.NamespaceUris.ToArray()[0].EndsWith("/")
            ? server.NamespaceUris.ToArray()[0] + iEntityObjectCreator.EntityName
            : server.NamespaceUris.ToArray()[0] + "/" + iEntityObjectCreator.EntityName;

        _iEntityObjectCreator = iEntityObjectCreator;
        _namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(_namespaceUri);
    }

    public NodeState EntityNode { get; private set; } = null!;


    /// <inheritdoc />
    protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
    {
        var root = CreateRootFolder();
        EntityNode = _iEntityObjectCreator.CreateEntityOpcUaNode(root, _namespaceIndex);
        return new NodeStateCollection([EntityNode]);
    }

    /// <inheritdoc />
    protected override void AddPredefinedNode(ISystemContext context, NodeState node)
    {
        base.AddPredefinedNode(context, node);
    }


    private NodeState CreateRootFolder()
    {
        var browseName = new QualifiedName(_iEntityObjectCreator.EntityName + "Content", _namespaceIndex);
        var root = new FolderState(null); // Parent is null for the root
        root.NodeId = new NodeId(Guid.NewGuid(), _namespaceIndex); //Give it a node id
        root.BrowseName = browseName;
        root.Description =
            new LocalizedText($"The root folder for my data about the {_iEntityObjectCreator.EntityName} entity.");
        root.ReferenceTypeId = ReferenceTypeIds.Organizes;
        root.TypeDefinitionId = ObjectTypeIds.FolderType;

        return root;
    }
}