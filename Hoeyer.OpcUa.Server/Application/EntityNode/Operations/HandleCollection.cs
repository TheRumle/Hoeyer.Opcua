using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class HandleCollection(IEntityNode entityNode)
{
    private readonly Dictionary<PropertyState, EntityNodeHandle<PropertyState>> _propertyHandles
        = entityNode.PropertyStates.ToDictionary(e => e.Value,
            e => new EntityNodeHandle<PropertyState>(e.Value, entityNode.Entity));

    public readonly EntityNodeHandle<BaseObjectState> EntityNodeHandle =
        new(entityNode.Entity, entityNode.Entity.Parent);
    
    public IReadOnlyDictionary<PropertyState, EntityNodeHandle<PropertyState>> PropertyHandles => _propertyHandles;

    private EntityNodeHandle<PropertyState> AddProperty(PropertyState state)
    {
        var handle = new EntityNodeHandle<PropertyState>(state, entityNode.Entity);
        _propertyHandles.Add(state, handle);
        return handle;
    }

    public EntityNodeHandle<PropertyState> GetOrCreatePropertyHandle(PropertyState state)
    {
        return _propertyHandles.TryGetValue(state, out var handle) ? handle : AddProperty(state);
    }
}