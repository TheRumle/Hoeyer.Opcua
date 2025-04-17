using System;

namespace Hoeyer.OpcUa.Client.Application.Events;

public sealed record EntityMonitoringConfiguration
{
    public TimeSpan WantedPublishingInterval { get; set; } = TimeSpan.FromSeconds(5);
}