using System.Diagnostics.Contracts;
using Opc.Ua;

namespace Hoeyer.OpcUa.Entity;

public interface IEntityNodeCreator
{
    public string EntityName { get; }

    [Pure]
    public EntityNode CreateEntityOpcUaNode(FolderState root, ushort assignedNamespace);
}