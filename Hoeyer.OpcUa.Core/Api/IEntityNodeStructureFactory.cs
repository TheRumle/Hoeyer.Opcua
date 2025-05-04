namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityNodeStructureFactory
{
    IEntityNode Create(ushort applicationNamespaceIndex);
}

public interface IEntityNodeStructureFactory<in T> : IEntityNodeStructureFactory;