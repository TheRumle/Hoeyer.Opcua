using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class AgentManager<T>(
    IManagedAgent<T> managedEntity,
    IServerInternal server,
    ILogger logger)
    : CustomNodeManager(server, managedEntity.Namespace), IAgentManager<T>
{
    public IManagedAgent ManagedEntity { get; } = managedEntity;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        using IDisposable? scope = logger.BeginScope(ManagedEntity.Select(e => e.ToLoggingObject()));
        logger.Log(LogLevel.Information, "Creating address space");
        try
        {
            InitializeNode(externalReferences);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create address space for entity");
        }
    }

    private void InitializeNode(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            BaseObjectState node = ManagedEntity.Select(e => e.BaseObject);
            AddPredefinedNode(SystemContext, node);
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out IList<IReference>? references))
            {
                references ??= new List<IReference>();
                externalReferences[ObjectIds.ObjectsFolder] = references;
            }

            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, node.NodeId));
        }
    }
}