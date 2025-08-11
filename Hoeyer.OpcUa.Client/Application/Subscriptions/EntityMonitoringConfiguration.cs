using System;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

public sealed record AgentMonitoringConfiguration
{
    public TimeSpan ServerPublishingInterval { get; set; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    ///     The amount of time that must pass without any state changes are reported to an agent before the agent is
    ///     considered stable and publishing of messages occurs.
    /// </summary>
    public TimeSpan AgentStabilisationTime { get; set; } = TimeSpan.FromMilliseconds(500);
}