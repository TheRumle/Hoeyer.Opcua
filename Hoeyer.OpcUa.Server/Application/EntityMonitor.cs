using System;
using System.Collections.Generic;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;


public sealed class EntityMonitor(ILogger logger ) : IEntityMonitor, IMessageConsumer<IEntityNode>, IDisposable
{
    public EntityMonitorResponse Subscribe(IEventMonitoredItem item, uint subscriptionId)
    {

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorResponse Unsubscribe(IEventMonitoredItem item, uint subscriptionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorResponse SubscribeToAll(IEventMonitoredItem item, uint subscriptionId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorResponse CreateMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorResponse ModifyMonitoredItem(IEnumerable<MonitoredItemCreateRequest> itemsToCreate, uint subscriptionId, TimeSpan publishingInterval)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public EntityMonitorDeleteResponse DeleteMonitor(IList<IMonitoredItem> monitoredItems)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ModeChangeResponse SetMonitoringMode(IList<IMonitoredItem> monitoredItems)
    {
        throw new NotImplementedException();
    }



    /// <inheritdoc />
    public void Dispose()
    { }

    public void Consume(IMessage<IEntityNode> message)
    {
        throw new NotImplementedException();
    }
}