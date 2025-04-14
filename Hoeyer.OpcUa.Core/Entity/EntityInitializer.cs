using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Entity;

[OpcUaEntityService(typeof(IEntityInitializer), ServiceLifetime.Singleton)]
public sealed class EntityInitializer<T>(
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory,
    IEntityChangedMessenger<T> messenger) : IEntityInitializer
{
    public string EntityName { get; } = typeof(T).Name;

    public async Task<(IEntityNode node, IMessagePublisher<IEntityNode> nodeChangedPublisher)> CreateNode(ushort namespaceIndex)
    {
        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        return (nodeRepresentation, messenger);
    }
}