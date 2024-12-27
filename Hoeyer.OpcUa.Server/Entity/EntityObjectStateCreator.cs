using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Variables;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity;

internal sealed class EntityObjectStateCreator<TEntity>(ISystemContext context, NodeState root)
    : IEntityObjectStateCreator
{
    private readonly string _name = typeof(TEntity).Name;
    
    public BaseObjectState CreateEntityOpcUaNode(ushort namespaceIndex)
    {
        var entityNode = CreateEntityNode(namespaceIndex);

        var readableProperties = typeof(TEntity).GetProperties()
            .Where(e => e.CanRead && e.GetAccessors().Any(accessor => accessor.IsPublic));
        
        foreach (var publicProperty in readableProperties)
        {
            AddVariable(publicProperty, entityNode,namespaceIndex);
        }
        
        return entityNode;
    }

    private BaseObjectState CreateEntityNode(ushort dynamicNamespaceIndex)
    {
        BaseObjectState entityNode = new BaseObjectState(root);
        entityNode.Create(context, root.NodeId, new QualifiedName(_name, dynamicNamespaceIndex), null, true);
        entityNode.BrowseName = new QualifiedName(_name, dynamicNamespaceIndex);
        root.AddChild(entityNode);
        return entityNode;
    }

    private void AddVariable(PropertyInfo property, BaseObjectState entity, ushort dynamicNamespaceIndex)
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