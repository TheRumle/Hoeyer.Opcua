using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal class HandleCollection(IEntityNode entityNode)
{
    private readonly Dictionary<PropertyState, PropertyHandle> _propertyHandles
        = entityNode.PropertyStates.ToDictionary(e => e.Value, PropertyHandle (e) => new PropertyHandle(e.Value));

    public readonly EntityHandle ManagedHandle = new(entityNode.Entity);

    public IReadOnlyDictionary<PropertyState, PropertyHandle> PropertyHandles => _propertyHandles;

    private IEntityNodeHandle AddProperty(PropertyState state)
    {
        var handle = new PropertyHandle(state);
        _propertyHandles.Add(state, handle);
        return handle;
    }

    public IEntityNodeHandle GetOrCreatePropertyHandle(PropertyState state)
    {
        return _propertyHandles.TryGetValue(state, out var handle) ? handle : AddProperty(state);
    }
}