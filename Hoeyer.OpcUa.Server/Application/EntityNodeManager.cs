using System.Collections.Generic;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;


internal sealed class EntityNodeManager<T>(
    IManagedEntityNode managedEntity,
    IServerInternal server,
    ILogger logger)
    : CustomNodeManager(server, managedEntity.Namespace), IEntityNodeManager<T>
{
    public IManagedEntityNode ManagedEntity { get; } = managedEntity;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        logger.BeginScope(ManagedEntity.ToLoggingObject());
        logger.Log(LogLevel.Information, "Creating address space");
        logger.TryAndReThrow(() =>
        {
            lock (Lock)
            {
                var node = ManagedEntity.BaseObject;
                AddPredefinedNode(SystemContext, ManagedEntity.BaseObject);
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, value: out var references))
                {
                    references ??= new List<IReference>();
                    externalReferences[ObjectIds.ObjectsFolder] = references;
                }
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, node.NodeId));
            }
        });
    }
}