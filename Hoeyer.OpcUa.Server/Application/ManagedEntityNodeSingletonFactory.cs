using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IManagedEntityNodeSingletonFactory<>), ServiceLifetime.Singleton)]
internal sealed class ManagedEntityNodeSingletonFactory<T>(
    IOpcUaEntityServerInfo info,
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory,
    IEntityChangedBroadcaster<T> broadcaster) : IManagedEntityNodeSingletonFactory<T>
{
    public IManagedEntityNode? Node { get; private set; }

    public async Task<IManagedEntityNode> CreateManagedEntityNode(Func<string, ushort> namespaceToIndex)
    {
        if (Node != null) return Node;

        var @namespace = info.Host + $"/{typeof(T).Name}";
        var namespaceIndex = namespaceToIndex.Invoke(@namespace);

        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        Node = new ManagedEntityNode(nodeRepresentation, @namespace, namespaceIndex);
        broadcaster.BeginObserve(Node);
        return Node;
    }
}