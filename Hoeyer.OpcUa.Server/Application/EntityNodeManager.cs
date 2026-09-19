using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class EntityNodeManager<T>(
    Uri applicationNamespaceUri,
    IManagedEntityNodeProvider<T> nodeProvider,
    IEnumerable<INodeConfigurator<T>> nodeConfigurators,
    ILogger<EntityNodeManager<T>> logger,
    IEntityNodeAccessConfigurator accessConfigurator,
    IServerInternal server)
    : CustomNodeManager(server, applicationNamespaceUri.ToString()),
        IEntityNodeManager<T>
{
    private readonly TaskCompletionSource<bool> _addressSpaceReady =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private Task<IManagedEntityNode<T>> _nodeTask = null!;
    private int _setupStarted;
    public IManagedEntityNode ManagedEntity { get; private set; } = null!;

    public Task NodeReady => _addressSpaceReady.Task;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        if (Interlocked.CompareExchange(ref _setupStarted, 1, 0) != 0)
        {
            var duplicateSetup = new DuplicateSetupException(typeof(T));
            logger.LogCritical(duplicateSetup,
                "Address space setup was attempted more than once for entity {EntityType}", typeof(T).Name);
            throw duplicateSetup;
        }

        try
        {
            using var scope = logger.BeginScope(nameof(CreateAddressSpace));
            logger.LogDebug("Creating managed entity node");

            _nodeTask = nodeProvider.GetOrCreateManagedEntityNode(NamespaceIndex, NamespaceUris.First());
            ManagedEntity = _nodeTask.Result;
            accessConfigurator.Configure(ManagedEntity, SystemContext);
            ManagedEntity.ChangeState(entity =>
            {
                ConfigureEntity(nodeConfigurators);
                AddEntityStructure(entity, externalReferences);
            });
            base.CreateAddressSpace(externalReferences);
            _addressSpaceReady.TrySetResult(true);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Failed to create address space for entity");
            _addressSpaceReady.TrySetException(e);
        }
    }

    private void ConfigureEntity(IEnumerable<INodeConfigurator<T>> enumerable)
    {
        foreach (var configurator in enumerable)
        {
            configurator.Configure(ManagedEntity, SystemContext);
        }
    }

    private void AddEntityStructure(
        IEntityNode entityNode,
        IDictionary<NodeId, IList<IReference>> externalReferences
    )
    {
        var wantedPlacement = ObjectIds.RootFolder;
        var node = entityNode.BaseObject;
        AddPredefinedNode(SystemContext, node);
        if (!externalReferences.TryGetValue(wantedPlacement, out var references))
        {
            references ??= new List<IReference>();
            externalReferences[wantedPlacement] = references;
        }

        references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, node.NodeId));
    }
}