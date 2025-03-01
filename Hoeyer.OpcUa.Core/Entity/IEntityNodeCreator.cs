using System.Diagnostics.Contracts;

namespace Hoeyer.OpcUa.Entity;

public interface IEntityNodeCreator
{
    public string EntityName { get; }

    [Pure]
    public EntityNode CreateEntityOpcUaNode(ushort assignedNamespace);
}