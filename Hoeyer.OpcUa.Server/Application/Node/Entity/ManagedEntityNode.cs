using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Node.Entity;

public sealed record ManagedEntityNode(BaseObjectState Entity, FolderState Folder, IEnumerable<PropertyState> PropertyStates, string Namespace, ushort EntityNameSpaceIndex) : IEntityNode
{
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; } = Folder;

    public IEnumerable<PropertyState> PropertyStates { get; } = PropertyStates;
    public string Namespace { get; } = Namespace;
    public ushort EntityNameSpaceIndex { get; } = EntityNameSpaceIndex;

    public ManagedEntityNode(IEntityNode node, string entityNamespace, ushort entityNamespaceIndex)
        :this(node.Entity, node.Folder, node.PropertyStates, entityNamespace, entityNamespaceIndex)
    {
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $$"""
                  {{nameof(ManagedEntityNode)}} { Entity:
                    Name: {{Entity.DisplayName}},
                    Id: {{Entity.NodeId}},
                    Namespace: {{EntityNameSpaceIndex}},
                    Folder: {{Folder.DisplayName}},
                    State: [
                        {{PropertyStates.Select(e=> $"{{{e.DisplayName}: {e.Value}, Id: \"{e.NodeId}\"").SeparatedBy(",\n")}}
                    ]
                  """;
    }
}