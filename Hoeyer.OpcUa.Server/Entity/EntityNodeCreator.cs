using System;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Nodes.Variables;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

internal sealed class EntityNodeCreator<TEntity> : IEntityNodeCreator
{
    public string EntityName { get; } =  typeof(TEntity).Name;
    
    public NodeState CreateEntityOpcUaNode(NodeState root, ushort namespaceIndex)
    {
        var entityNode = CreateEntityNode(root, namespaceIndex);

        var readableProperties = typeof(TEntity).GetProperties()
            .Where(e => e.CanRead && e.GetAccessors().Any(accessor => accessor.IsPublic));
        
        foreach (var publicProperty in readableProperties)
        {
            AddVariable(publicProperty, entityNode, namespaceIndex);
        }
        
        return entityNode;
    }

    private NodeState CreateEntityNode(NodeState root, ushort dynamicNamespaceIndex)
    {
        BaseObjectState entityNode = new BaseObjectState(root)
        {
            BrowseName =  new QualifiedName(EntityName, dynamicNamespaceIndex),
            NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex),
            DisplayName = EntityName,
        };

        root.AddChild(entityNode);
        return entityNode;
    }

    private static void AddVariable(PropertyInfo property, NodeState entity, ushort dynamicNamespaceIndex)
    {
        //TODO here is a good place to use source generation
        var  (dataTypeId, rank) = SupportedVariableTypeCallbackHandler.ToDataTypeId(property);
        PropertyState? a = entity.AddProperty<int>(property.Name, dataTypeId, ValueRanks.Scalar);
        a.NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex);
    }
}