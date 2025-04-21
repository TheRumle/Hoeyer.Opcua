using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Server.Api.Monitoring;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Monitoring;

public sealed class MonitoredPropertyForwarder(Func<IEnumerable<MonitoredProperty>> monitoredProperties) :  IStateChangeConsumer<PropertyState, object>
{
    private readonly UniqueByProperty _uniqueByProperty = new();
    private sealed class UniqueByProperty : IEqualityComparer<StateChange<PropertyState, object>>
    {
        public bool Equals(StateChange<PropertyState, object> x, StateChange<PropertyState, object> y)
        {
            return Equals(x.NewValue, y.NewValue) && Equals(x.State, y.State) && Equals(x.OldValue, y.OldValue);
        }

        public int GetHashCode(StateChange<PropertyState, object> obj) => obj.State.GetHashCode();
    }
    
    public void Consume(IMessage<IEnumerable<StateChange<PropertyState, object>>> changedProperties)
    {
        var payload = changedProperties.Payload.ToHashSet(_uniqueByProperty);
        foreach (MonitoredProperty item in monitoredProperties.Invoke().Where(IsItemForDiscreteDataEvent))
        {
            
        }
    }
    
    public static bool IsItemForDiscreteDataEvent(MonitoredProperty e)
    {
        int type = e.Item.MonitoredItemType;
        return (type & (MonitoredItemTypeMask.DataChange | MonitoredItemTypeMask.AllEvents)) != 0 && e.Item.MonitoringMode == MonitoringMode.Reporting;
    }
}