namespace Hoeyer.OpcUa.Core.Abstractions;

public interface IEntityNodeStructureFactory
{
    IEntityNode Create(ushort applicationNamespaceIndex);
}

public interface IEntityNodeStructureFactory<in T> : IEntityNodeStructureFactory;