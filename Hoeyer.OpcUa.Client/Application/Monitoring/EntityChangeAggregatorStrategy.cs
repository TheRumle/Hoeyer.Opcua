using System;
using System.Threading;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

[OpcUaEntityService(typeof(IEntityChangeAggregatorStrategy<>))]
public sealed class EntityChangeAggregatorStrategy<T>(
    EntityMonitoringConfiguration configuration,
    IEntityBrowser<T> browser) : IEntityChangeAggregatorStrategy<T>
{
    private IEntityNode _node { get; set; }

    /// <inheritdoc />
    public void Register(MonitoredItemNotification item)
    {
        _node ??= browser.BrowseEntityNode(CancellationToken.None).Result;

        TimeSpan _ = configuration.EntityStabilisationTime;
    }
}