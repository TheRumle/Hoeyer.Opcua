using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.Management;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application.Management;

[OpcUaEntityService(typeof(IEntityServiceContainer), ServiceLifetime.Singleton)]
public sealed class EntityServiceContainer<T>(
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory,
    IEntityChangedMessenger<T> messenger) : IEntityServiceContainer
{
    /// <inheritdoc />
    public IMessagePublisher<IEntityNode> EntityChangedPublisher => messenger;
    public string EntityName { get; } = typeof(T).Name;

    public async Task<IEntityNode> CreateNode(ushort namespaceIndex)
    {
        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        return nodeRepresentation;
    }
}