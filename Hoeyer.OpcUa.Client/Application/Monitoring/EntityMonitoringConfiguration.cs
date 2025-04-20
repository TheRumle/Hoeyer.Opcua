using System;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

public sealed record EntityMonitoringConfiguration
{
    public TimeSpan WantedPublishingInterval { get; set; } = TimeSpan.FromSeconds(5);
}