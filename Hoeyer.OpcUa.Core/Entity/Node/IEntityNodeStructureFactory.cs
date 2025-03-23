namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeStructureFactory
{
    public string EntityName { get; }
    IEntityNode Create(ushort applicationNamespaceIndex);
}

public interface IEntityNodeStructureFactory<in T> : IEntityNodeStructureFactory;