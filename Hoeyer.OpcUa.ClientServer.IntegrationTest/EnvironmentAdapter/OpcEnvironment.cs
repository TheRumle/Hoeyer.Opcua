using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

namespace Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;

public sealed record OpcEnvironment
{
    private const string SERVER_ID = "MyServer";
    private const string SERVER_NAME = "MyServer";
    public required WebProtocol Protocol { get; init; }
    public required string OpcUaServerId { get; init; }
    public required string OpcUaServerName { get; init; }
    public required string HostName { get; init; }
    public required int Port { get; init; }

    public static OpcEnvironment Default(int port, string hostName) =>
        new()
        {
            HostName = hostName,
            Port = port,
            OpcUaServerId = SERVER_ID,
            OpcUaServerName = SERVER_NAME,
            Protocol = WebProtocol.OpcTcp
        };
}