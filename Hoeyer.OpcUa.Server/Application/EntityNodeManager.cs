using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
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
    : CustomNodeManager(server, applicationNamespaceUri.ToString()), IEntityNodeManager<T>
{
    public IManagedEntityNode ManagedEntity { get; private set; } = null!;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        try
        {
            using var scope = logger.BeginScope(nameof(CreateAddressSpace));
            logger.LogDebug("Creating managed entity node");

            var nodeTask = nodeProvider.GetOrCreateManagedEntityNode(NamespaceIndex, NamespaceUris.First());
            ManagedEntity = nodeTask.Result;
            accessConfigurator.Configure(ManagedEntity, SystemContext);
            ManagedEntity.ChangeState(entity =>
            {
                ConfigureEntity(nodeConfigurators);
                AddEntityStructure(entity, externalReferences);
            });
            base.CreateAddressSpace(externalReferences);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create address space for entity");
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