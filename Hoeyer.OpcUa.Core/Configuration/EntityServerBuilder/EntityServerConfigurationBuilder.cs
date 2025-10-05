using System;
using System.Globalization;
using Hoeyer.Common.Extensions.Collection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

internal class EntityServerConfigurationBuilder : IEntityServerConfigurationBuilder, IServerNameStep, IWithOriginsStep,
    IWithApplicationUri
{
    private Uri _applicationUri = null!;
    private Uri _host = null!;
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;


    private EntityServerConfigurationBuilder()
    {
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

    public IOpcUaTargetServerInfo Build()
    {
        if (!_host.IsBaseOf(_applicationUri))
        {
            throw new ArgumentException($"The host uri {_host} is not a base of application {_applicationUri}");
        }

        return new OpcUaTargetServerInfo(_serverId, _serverName, _host, _applicationUri);
    }

    /// <inheritdoc />
    public IWithApplicationUri WithApplicationUri(string applicationNameUri)
    {
        _applicationUri = ParseUri(_host + ParseAdditionalPath(applicationNameUri));
        return this;
    }

    /// <inheritdoc />
    public IWithApplicationUri WithWebOrigins(WebProtocol protocol, string host, int port)
    {
        var protocolsString = protocol switch
        {
            WebProtocol.OpcTcp => Utils.UriSchemeOpcTcp,
            WebProtocol.Https => Utils.UriSchemeHttps,
            WebProtocol.WebSocketSecure => Utils.UriSchemeOpcWss,
            var _ => throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null)
        };

        var uriString = $"{protocolsString}://{host}:{port}";
        _host = ParseUri(uriString);
        _applicationUri = _host;
        return this;
    }

    public static IEntityServerConfigurationBuilder Create()
    {
        return new EntityServerConfigurationBuilder();
    }

    private static Uri ParseUri(string uriString)
    {
        var isValidUri = Uri.TryCreate(string.Format(CultureInfo.InvariantCulture, "{0}", uriString),
            UriKind.RelativeOrAbsolute, out var result);

        if (!isValidUri)
        {
            throw new ArgumentException($"Host and serverId could not form a valid URI: {uriString}");
        }

        return result;
    }

    private string ParseAdditionalPath(string additionalPath)
    {
        if (additionalPath.IsEmpty())
        {
            return additionalPath;
        }

        if (additionalPath.Length > 1 && additionalPath.StartsWith('/'))
        {
            return additionalPath.Substring(1);
        }

        if (additionalPath.Length >= 1 && !additionalPath.StartsWith('/'))
        {
            return "/" + additionalPath;
        }

        return additionalPath;
    }
}