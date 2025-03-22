namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeFactory;
public interface IEntityNodeFactory<in T> : IEntityNodeFactory
{
    IEntityNode Create(T state, ushort applicationNamespaceIndex);
}