using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityMonitor
{
    public EntityMonitorResponse Subscribe(IEventMonitoredItem item, uint subscriptionId);
    public EntityMonitorResponse Unsubscribe(IEventMonitoredItem item, uint subscriptionId);
    /// <summary>
    /// Only subscription to data change is allowed
    /// </summary>
    public EntityMonitorResponse SubscribeToAll(IEventMonitoredItem item, uint subscriptionId); 
    public EntityMonitorResponse CreateMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval); 
    
    public EntityMonitorResponse ModifyMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval);

    public EntityMonitorDeleteResponse DeleteMonitor(IList<IMonitoredItem> monitoredItems);
    public ModeChangeResponse SetMonitoringMode(IList<IMonitoredItem> monitoredItems);
}