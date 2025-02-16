using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public sealed record EntityNode(FolderState Folder, BaseObjectState Entity, IEnumerable<PropertyState> PropertyStates)
{
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; set; } = Folder;

    public IEnumerable<PropertyState> PropertyStates { get; } =  PropertyStates;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Folder: {Folder.DisplayName} containing {Entity.DisplayName} with NodeId {Entity.NodeId}";
    }
}