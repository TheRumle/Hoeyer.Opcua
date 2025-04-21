using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.RequestResponse;

namespace Hoeyer.OpcUa.Server.Api.Management;

public sealed record ServiceContainer(IEntityNode EntityNode, IEntityChangedBroadcaster Publisher)
{
    public IEntityNode EntityNode { get; } = EntityNode;
    public IEntityChangedBroadcaster Publisher { get; } = Publisher;
}

public interface IEntityServiceContainerFactory
{
    public string EntityName { get; }
    public Task<ServiceContainer> CreateServiceContainer(ushort namespaceIndex);
}