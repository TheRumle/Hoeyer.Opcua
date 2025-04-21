using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api.Monitoring;

public readonly record struct MonitoredProperty(ManagedHandle<PropertyState> State, IMonitoredItem Item)
{
    public IMonitoredItem Item { get; } = Item;
    public ManagedHandle<PropertyState> State { get; } = State;
}