using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public interface IEntityNode
{
    public BaseObjectState Entity { get; }
    public FolderState Folder { get; }

    public IEnumerable<PropertyState> PropertyStates { get; }
}

public sealed record EntityNode(FolderState Folder, BaseObjectState Entity, IEnumerable<PropertyState> PropertyStates)
    : IEntityNode
{
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; } = Folder;

    public IEnumerable<PropertyState> PropertyStates { get; } =  PropertyStates;
}