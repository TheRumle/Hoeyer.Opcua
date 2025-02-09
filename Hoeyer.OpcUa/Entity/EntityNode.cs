using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public sealed record EntityNode(FolderState Folder, NodeState Entity, IEnumerable<PropertyState> PropertyStates)
{
    public NodeState Entity { get; } = Entity;
    public FolderState Folder { get; set; } = Folder;

    public IEnumerable<PropertyState> PropertyStates { get; } =  PropertyStates;
}