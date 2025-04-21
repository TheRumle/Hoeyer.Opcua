using System.Threading.Tasks;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.Management;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application.Management;

[OpcUaEntityService(typeof(IEntityServiceContainerFactory), ServiceLifetime.Singleton)]
public sealed class EntityServiceContainerFactory<T>(
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory,
    IEntityChangedBroadcaster<T> entityChangedBroadcaster) : IEntityServiceContainerFactory
{
    public string EntityName { get; } = typeof(T).Name;

    public async Task<ServiceContainer> CreateServiceContainer(ushort namespaceIndex)
    {
        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        return new ServiceContainer(nodeRepresentation, entityChangedBroadcaster);
    }
}