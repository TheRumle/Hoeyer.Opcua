using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaAgentService(typeof(IManagedAgentSingletonFactory<>), ServiceLifetime.Singleton)]
internal sealed class ManagedAgentSingletonFactory<T>(
    IOpcUaAgentServerInfo info,
    IAgentLoader<T> value,
    IAgentTranslator<T> translator,
    IAgentStructureFactory<T> structureFactory) : IManagedAgentSingletonFactory<T>
{
    private IManagedAgent<T>? _node;
    public IManagedAgent? Node => _node;

    public async Task<IManagedAgent<T>> CreateManagedAgent(Func<string, ushort> namespaceToIndex)
    {
        if (_node != null) return _node;

        var @namespace = info.Host + $"/{typeof(T).Name}";
        var namespaceIndex = namespaceToIndex.Invoke(@namespace);

        var agent = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(agent, nodeRepresentation);
        _node = new ManagedAgent<T>(nodeRepresentation, @namespace, namespaceIndex);
        return _node;
    }

    /// <inheritdoc />
    public IManagedAgent<T> GetAgent()
    {
        if (_node == null)
        {
            throw new AgentProviderException(typeof(T),
                $"The node factory has not yet been provided with an namespace index used to construct the node. Have you awaited the {nameof(AgentServerStartedMarker)}?");
        }

        return _node;
    }
}