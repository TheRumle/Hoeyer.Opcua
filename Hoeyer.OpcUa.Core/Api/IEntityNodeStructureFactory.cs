namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityNodeStructureFactory<in T>
{
    IEntityNode Create(ushort applicationNamespaceIndex);
}