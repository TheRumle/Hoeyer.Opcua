﻿using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

internal sealed record ManagedEntityNode(
    BaseObjectState Entity,
    FolderState Folder,
    Dictionary<NodeId, PropertyState> PropertyStates,
    string Namespace,
    ushort EntityNameSpaceIndex) : IEntityNode
{
    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
        : this(node.Entity, node.Folder, node.PropertyStates, entityNamespace, entityNamespaceIndex)
    {
    }

    public string Namespace { get; } = Namespace;
    public ushort EntityNameSpaceIndex { get; } = EntityNameSpaceIndex;
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; } = Folder;

    public Dictionary<NodeId, PropertyState> PropertyStates { get; } = PropertyStates;

    /// <inheritdoc />
    public override string ToString()
    {
        return $$"""
                 {{nameof(ManagedEntityNode)}} {
                   Name: {{Entity.DisplayName}},
                   Id: {{Entity.NodeId}},
                   Namespace: {{EntityNameSpaceIndex}},
                   Folder: {{Folder.DisplayName}},
                   State: [
                       {{PropertyStates.Values.Select(e => $"{{{e.DisplayName}: {e.Value}, Id: \"{e.NodeId}\"}}").SeparatedBy(",\n")}}
                   ]
                 }
                 """;
    }
}