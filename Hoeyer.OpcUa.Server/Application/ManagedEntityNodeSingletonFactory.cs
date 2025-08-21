using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class ManagedEntityNodeSingletonFactory<T>(
    IOpcUaEntityServerInfo info,
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory) : IManagedEntityNodeSingletonFactory<T>
{
    private IManagedEntityNode<T>? _node;
    public IManagedEntityNode? Node => _node;

    public async Task<IManagedEntityNode<T>> CreateManagedEntityNode(Func<string, ushort> namespaceToIndex)
    {
        if (_node != null) return _node;

        var @namespace = info.Host + $"/{typeof(T).Name}";
        var namespaceIndex = namespaceToIndex.Invoke(@namespace);

        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        _node = new ManagedEntityNode<T>(nodeRepresentation, @namespace, namespaceIndex);
        return _node;
    }

    /// <inheritdoc />
    public IManagedEntityNode<T> GetEntityNode()
    {
        if (_node == null)
        {
            throw new EntityNodeProviderException(typeof(T),
                $"The node factory has not yet been provided with an namespace index used to construct the node. Have you awaited the {nameof(EntityServerStartedMarker)}?");
        }

        return _node;
    }
}