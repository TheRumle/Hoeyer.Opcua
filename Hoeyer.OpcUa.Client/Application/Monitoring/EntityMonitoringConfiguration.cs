using System;

namespace Hoeyer.OpcUa.Client.Application.Monitoring;

public sealed record EntityMonitoringConfiguration
{
    public TimeSpan ServerPublishingInterval { get; set; } = TimeSpan.FromMilliseconds(50);
    
    /// <summary>
    /// The amount of time that must pass without any state changes are reported to an entity before the entity is considered stable and publishing of messages occurs. 
    /// </summary>
    public TimeSpan EntityStabilisationTime { get; set; } = TimeSpan.FromMilliseconds(500);
}