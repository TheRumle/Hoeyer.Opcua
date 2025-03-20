using System.Diagnostics.Contracts;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeCreator<out T> : IEntityNodeCreator
{
    public T RepresentedEntity { get; }
}

public interface IEntityNodeCreator
{
    public string EntityName { get; }

    [Pure]
    public IEntityNode CreateEntityOpcUaNode(ushort assignedNamespace);
}