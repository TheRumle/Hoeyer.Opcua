using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

internal record OpcUaEntityServerInfo : IOpcUaEntityServerInfo
{
    private readonly Uri _host;

    public OpcUaEntityServerInfo(string serverId, string serverName, Uri applicationNamespace)
    {
        Uri.TryCreate(applicationNamespace.ToString(), UriKind.Absolute, out _host);
        ValidateSupportedProtocol([Host]);

        ServerId = serverId;
        ApplicationName = serverName;
        ApplicationNamespace = applicationNamespace;
        OpcUri = new UriBuilder(Host)
        {
            Scheme = Host.Scheme,
            Port = Host.Port
        }.Uri;
    }

    public Uri Host => _host;

    public string ServerId { get; }
    public string ApplicationName { get; }

    /// <summary>
    ///     For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; }

    /// <inheritdoc />
    public Uri OpcUri { get; }

    private static void ValidateSupportedProtocol(IEnumerable<Uri> addresses)
    {
        var errors = addresses.Select(address => address.Scheme switch
            {
                Utils.UriSchemeHttps or Utils.UriSchemeOpcHttps => null,
                Utils.UriSchemeOpcTcp => null,
                Utils.UriSchemeOpcWss => null,
                "http" => "HTTP is not supported, use https instead",
                var _ => $"Protocol {address.Scheme} is not supported"
            })
            .Where(e => e is not null)
            .ToArray();

        if (errors.Length > 0) throw new InvalidServerConfigurationException(string.Join("\n", errors));
    }
}