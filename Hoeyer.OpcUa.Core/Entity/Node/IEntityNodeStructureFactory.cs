namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeStructureFactory
{
    IEntityNode Create(ushort applicationNamespaceIndex);
}

public interface IEntityNodeStructureFactory<in T> : IEntityNodeStructureFactory;