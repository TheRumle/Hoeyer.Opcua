namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeFactory
{
    public string EntityName { get; }
    IEntityNode Create(ushort applicationNamespaceIndex);
}

public interface IEntityNodeFactory<in T> : IEntityNodeFactory;