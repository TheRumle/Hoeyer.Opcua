using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;

public sealed record EntityNode(FolderState Folder, NodeState Entity)
{
    public NodeState Entity { get; } = Entity;
    public FolderState Folder { get; } = Folder;
    
}