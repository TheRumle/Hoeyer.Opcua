using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Variables;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

internal sealed class EntityObjectStateCreator<TEntity> : IEntityObjectStateCreator
{
    public string EntityName { get; } =  typeof(TEntity).Name;
    
    public BaseObjectState CreateEntityOpcUaNode(ISystemContext context, NodeState root, ushort namespaceIndex)
    {
        var entityNode = CreateEntityNode(context, root, namespaceIndex);

        var readableProperties = typeof(TEntity).GetProperties()
            .Where(e => e.CanRead && e.GetAccessors().Any(accessor => accessor.IsPublic));
        
        foreach (var publicProperty in readableProperties)
        {
            AddVariable(publicProperty, entityNode, namespaceIndex, context);
        }
        
        return entityNode;
    }

    private BaseObjectState CreateEntityNode(ISystemContext context, NodeState root, ushort dynamicNamespaceIndex)
    {
        BaseObjectState entityNode = new BaseObjectState(root);
        entityNode.Create(context, root.NodeId, new QualifiedName(EntityName, dynamicNamespaceIndex), EntityName, true);
        entityNode.BrowseName = new QualifiedName(EntityName, dynamicNamespaceIndex);
        root.AddChild(entityNode);
        return entityNode;
    }

    private void AddVariable(PropertyInfo property, BaseObjectState entity, ushort dynamicNamespaceIndex,
        ISystemContext context)
    {
        var name = property.Name;
        BaseDataVariableState speedNode = new BaseDataVariableState(entity);
        speedNode.AccessLevel = AccessLevels.CurrentReadOrWrite;
        speedNode.Create(context, entity.NodeId, new QualifiedName(name, dynamicNamespaceIndex), name, true);
        speedNode.BrowseName = new QualifiedName(name, dynamicNamespaceIndex);
        
        speedNode.DataType = property.PropertyType.ToOpcDataType();
        entity.AddChild(speedNode);
    }
}