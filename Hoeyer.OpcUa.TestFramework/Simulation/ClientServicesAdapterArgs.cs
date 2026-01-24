using Hoeyer.OpcUa.Core.Configuration.ServerTarget;

namespace Hoeyer.OpcUa.Test.Simulation;

internal readonly record struct ClientServicesAdapterArgs
{
    public required WebProtocol Protocol { get; init; }
    public required string OpcUaServerId { get; init; }
    public required string OpcUaServerName { get; init; }
    public required string HostName { get; init; }
    public required int Port { get; init; }
}