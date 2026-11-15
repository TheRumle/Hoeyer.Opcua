using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Abstractions.Configuration;

/// <summary>
/// Contains details on how the application is configured
/// </summary>
/// <param name="OpcUaApplicationConfiguration">The configuration details used to configure the inner OpcUa server details. Will always have <see cref="ApplicationType.Server"/>. See <see cref="ApplicationConfiguration"/></param> for more details
public sealed record ServerApplicationConfiguration(
    ApplicationConfiguration OpcUaApplicationConfiguration
)
{
    public ServerConfigurable GetServerConfigurable() => new(OpcUaApplicationConfiguration.ServerConfiguration!);
}