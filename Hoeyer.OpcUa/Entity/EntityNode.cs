using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public sealed record EntityNode(FolderState Folder, BaseObjectState Entity, IEnumerable<PropertyState> PropertyStates)
{
    public BaseObjectState Entity { get; } = Entity;
    public FolderState Folder { get; set; } = Folder;

    public IEnumerable<PropertyState> PropertyStates { get; } =  PropertyStates;

    public string? Namespace { get; set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        var namespaceText = Namespace == null ? "No Namespace assigned" : Namespace;
        return $@"Namespace ""{namespaceText}"" with folder ""{Folder.DisplayName}"" containing entity ""{Entity.DisplayName}"" with NodeId ""{Entity.NodeId}""";
    }
}