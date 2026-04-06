using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class ManagedEntityNodeProvider<T>(
    IOpcUaTargetServerInfo info,
    IEntityLoader<T> value,
    IEntityTranslator<T> translator,
    IEntityNodeStructureFactory<T> structureFactory) : IManagedEntityNodeProvider<T>
{
    private IManagedEntityNode<T>? _node;
    public IManagedEntityNode<T> Node => GetEntityNode();

    public async Task<IManagedEntityNode<T>> GetOrCreateManagedEntityNode(ushort namespaceIndex, string @namespace)
    {
        if (_node != null) return _node;
        var entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        _node = new ManagedEntityNode<T>(nodeRepresentation, @namespace, namespaceIndex);
        return _node;
    }

    private IManagedEntityNode<T> GetEntityNode()
    {
        if (_node == null)
        {
            throw new EntityNodeProviderException(typeof(T),
                $"The node factory has not yet been provided with an namespace index used to construct the node. Have you awaited the {nameof(HealthCheck)}?");
        }

        return _node;
    }
}