using System;
using System.Globalization;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

internal class EntityServerConfigurationBuilder : IEntityServerConfigurationBuilder, IServerNameStep, IWithOriginsStep,
    IEntityServerConfigurationBuildable
{
    private Uri _host = null!;
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;

    private EntityServerConfigurationBuilder()
    {
    }

    public IOpcUaTargetServerInfo Build()
    {
        var validuri = Uri.TryCreate(string.Format(CultureInfo.InvariantCulture, "{0}", _host),
            UriKind.RelativeOrAbsolute, out var uri);
        if (!validuri)
        {
            throw new ArgumentException($"Host and serverId could not form a valid URI: {uri}");
        }

        return new OpcUaTargetServerInfo(_serverId, _serverName, uri);
    }

    public IServerNameStep WithServerId(string serverId)
    {
        _serverId = serverId;
        return this;
    }

    public IWithOriginsStep WithServerName(string serverName)
    {
        _serverName = serverName;
        return this;
    }

    /// <inheritdoc />
    public IEntityServerConfigurationBuildable WithWebOrigins(WebProtocol protocol, string host, int port)
    {
        var protocolsString = protocol switch
        {
            WebProtocol.OpcTcp => Utils.UriSchemeOpcTcp,
            WebProtocol.Https => Utils.UriSchemeHttps,
            WebProtocol.WebSocketSecure => Utils.UriSchemeOpcWss,
            var _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };

        _host = new Uri($"{protocolsString}://{host}:{port}");
        return this;
    }

    public static IEntityServerConfigurationBuilder Create()
    {
        return new EntityServerConfigurationBuilder();
    }
}