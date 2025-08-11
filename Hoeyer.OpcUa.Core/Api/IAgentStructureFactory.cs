namespace Hoeyer.OpcUa.Core.Api;

public interface IAgentStructureFactory<in T>
{
    IAgent Create(ushort applicationNamespaceIndex);
}