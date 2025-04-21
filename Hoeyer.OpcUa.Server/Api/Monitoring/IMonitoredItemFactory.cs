using System;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Server.Api.Management;
using Opc.Ua;
using Opc.Ua.Server;
using MonitoredItem = Opc.Ua.Server.MonitoredItem;
using Range = Opc.Ua.Range;

namespace Hoeyer.OpcUa.Server.Api.Monitoring;

public interface IMonitoredItemFactory
{
    IMonitoredItem CreateMonitoredItem(IOperationContext context, MonitoredItemCreateRequest request, uint subscriptionId);
}


public sealed class MonitoredItemFactory(Func<IServerInternal> serverCopy, Func<IEntityNodeManager> manager)
    : IMonitoredItemFactory
{
    private readonly IEntityNodeManager _manager = manager.Invoke();

    private static readonly Random Random = new Random();
    /// <inheritdoc />
    public IMonitoredItem CreateMonitoredItem(IOperationContext context, MonitoredItemCreateRequest request, uint subscriptionId)
    {

        return new MonitoredItem(
            serverCopy.Invoke(),
            _manager,
            _manager.HandleManager.EntityHandle,
            subscriptionId,
            Random.UInt(),
            request.ItemToMonitor,
            context.DiagnosticsMask,
            TimestampsToReturn.Source,
            request.MonitoringMode,
            request.RequestedParameters.ClientHandle,
            null,
            null,
            new Range(double.MinValue, double.MaxValue),
            0, 
            10,
            request.RequestedParameters.DiscardOldest,
            0);
    }
}