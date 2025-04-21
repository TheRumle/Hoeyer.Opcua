using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Api;

public interface IMonitoredItemManager
{
    public MonitorItemCreateResponse Subscribe(IEventMonitoredItem item, uint subscriptionId);
    public MonitorItemCreateResponse Unsubscribe(IEventMonitoredItem item, uint subscriptionId);
    /// <summary>
    /// Only subscription to data change is allowed
    /// </summary>
    public MonitorItemCreateResponse SubscribeToAll(IEventMonitoredItem item, uint subscriptionId); 
    public IEnumerable<MonitorItemCreateResponse> CreateMonitoredItem(IOperationContext context, IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval); 
    
    public MonitorItemCreateResponse ModifyMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval);

    public EntityMonitorDeleteResponse DeleteMonitor(IList<IMonitoredItem> monitoredItems);
    public ModeChangeResponse SetMonitoringMode(IList<IMonitoredItem> monitoredItems);
}