using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.Monitoring;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.Monitoring;

public sealed class MonitoredEntityItemsManager : IMonitoredItemManager, IDisposable
{
    private readonly IEntityHandleManager _entityHandleManager;
    private readonly IMonitoredItemFactory _factory;
    private readonly ConcurrentDictionary<uint, ConcurrentDictionary<uint, MonitoredProperty>> _subscriptions = new();
    private readonly IMessageSubscription<IEnumerable<StateChange<PropertyState, object>>> _entityChangeSubscription;
    public MonitoredEntityItemsManager(
        IEntityChangedBroadcaster changes,
        IEntityHandleManager entityHandleManager,
        IMonitoredItemFactory factory)
    {
        _entityHandleManager = entityHandleManager;
        _factory = factory;
        _entityChangeSubscription = changes.Subscribe( new MonitoredPropertyForwarder(() => _subscriptions.Values.SelectMany(e => e.Values)));
    }

    public MonitorItemCreateResponse Subscribe(IEventMonitoredItem item, uint subscriptionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public MonitorItemCreateResponse Unsubscribe(IEventMonitoredItem item, uint subscriptionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public MonitorItemCreateResponse SubscribeToAll(IEventMonitoredItem item, uint subscriptionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerable<MonitorItemCreateResponse> CreateMonitoredItem(IOperationContext context, IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval)
    {
        
        foreach (var (request, index) in itemsToCreate.Select((element, index)=> (i: element, e: index)))
        {
            if (request.MonitoringMode != MonitoringMode.Reporting)
            {
                yield return MonitorItemCreateResponse.NotSupported(index, request, "Only reporting is supported.");
            }
            if (!_entityHandleManager.IsManagedPropertyHandle(request.ItemToMonitor.NodeId, out var propertyHandle))
            {
                yield return MonitorItemCreateResponse.NotSupported(index, request, "No property exists with the given ID");
            }

            var item = _factory.CreateMonitoredItem(context, request, subscriptionId);
            _subscriptions[subscriptionId][item.Id] = new MonitoredProperty(propertyHandle, item);
            yield return MonitorItemCreateResponse.Ok(index, request, item);
        }
    }

    /// <inheritdoc />
    public MonitorItemCreateResponse ModifyMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorDeleteResponse DeleteMonitor(IList<IMonitoredItem> monitoredItems)
    {
        foreach (var subscription in _subscriptions.Values)
        {
            foreach (var item in monitoredItems)
            {
                subscription.TryRemove(item.Id, out _);
            }
        }

        return null!;
    }

    /// <inheritdoc />
    public ModeChangeResponse SetMonitoringMode(IList<IMonitoredItem> monitoredItems)
    {
        foreach (var item in monitoredItems)
        { 
            
            
        }
        throw new NotImplementedException();
    }
    
    public void Dispose() => _entityChangeSubscription.Dispose();
    
 
}