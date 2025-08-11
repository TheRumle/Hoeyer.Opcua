using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(IManagedAgentSingletonFactory<>), ServiceLifetime.Singleton)]
internal sealed class ManagedAgentSingletonFactory<T>(
    IOpcUaEntityServerInfo info,
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IAgentStructureFactory<T> structureFactory) : IManagedAgentSingletonFactory<T>
{
    private IManagedAgent<T>? _node;
    public IManagedAgent? Node => _node;

    public async Task<IManagedAgent<T>> CreateManagedAgent(Func<string, ushort> namespaceToIndex)
    {
        if (_node != null) return _node;

        var @namespace = info.Host + $"/{typeof(T).Name}";
        var namespaceIndex = namespaceToIndex.Invoke(@namespace);

        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        _node = new ManagedAgent<T>(nodeRepresentation, @namespace, namespaceIndex);
        return _node;
    }

    /// <inheritdoc />
    public IManagedAgent<T> GetAgent()
    {
        if (_node == null)
        {
            throw new AgentProviderException(typeof(T),
                $"The node factory has not yet been provided with an namespace index used to construct the node. Have you awaited the {nameof(EntityServerStartedMarker)}?");
        }

        return _node;
    }
}