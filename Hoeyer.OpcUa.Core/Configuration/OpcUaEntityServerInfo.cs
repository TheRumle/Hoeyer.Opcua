using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration;

internal record OpcUaAgentServerInfo : IOpcUaAgentServerInfo
{
    public OpcUaAgentServerInfo(string ServerId, string ServerName, Uri Host, ISet<Uri> Endpoints,
        Uri ApplicationNamespace)
    {
        ValidateSupportedProtocol([Host, ..Endpoints]);

        this.ServerId = ServerId;
        ApplicationName = ServerName;
        this.Host = Host;
        this.Endpoints = Endpoints;
        this.ApplicationNamespace = ApplicationNamespace;
        OpcUri = new UriBuilder(Host)
        {
            Scheme = "opc.tcp",
            Port = Host.Port // Ensure the port remains unchanged
        }.Uri;
    }

    public string ServerId { get; }
    public string ApplicationName { get; }
    public Uri Host { get; }
    public ISet<Uri> Endpoints { get; set; }

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
            _ => $"Protocol {address.Scheme} is not supported, use https instead"
        })
        .Where(e => e is not null)
        .ToArray();

        if (errors.Length > 0) throw new InvalidServerConfigurationException(string.Join("\n", errors));
    }
}