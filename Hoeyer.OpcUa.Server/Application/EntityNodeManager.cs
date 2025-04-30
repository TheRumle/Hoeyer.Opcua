using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;


internal sealed class EntityNodeManager<T>(
    IManagedEntityNode managedEntity,
    IServerInternal server)
    : CustomNodeManager(server, managedEntity.Namespace), IEntityNodeManager<T>
{
    public IEntityNode ManagedEntity { get; } = managedEntity;

    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        lock (Lock)
        {
            var node = ManagedEntity.BaseObject;
            AddPredefinedNode(SystemContext, ManagedEntity.BaseObject);
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, value: out var references))
            {
                references = new List<IReference>();
                externalReferences[ObjectIds.ObjectsFolder] = references;
            }
            references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, node.NodeId));
        }
    }
    
    /// <inheritdoc />
    public override void Write(OperationContext context, IList<WriteValue> nodesToWrite, IList<ServiceResult> errors)
    {
        base.Write(context, nodesToWrite, errors);
    }
}