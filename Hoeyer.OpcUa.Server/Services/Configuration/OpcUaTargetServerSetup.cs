using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services.Configuration;

public sealed record OpcUaTargetServerSetup : IOpcUaTargetServerSetup
{
    public OpcUaTargetServerSetup(IOpcUaTargetServerInfo root, AdditionalServerConfiguration additionalConfiguration)
        : this(root.ServerId, root.ApplicationName, root.Host, new HashSet<Uri>([root.Host]), root.ApplicationNamespace,
            additionalConfiguration.Invoke)
    {
    }

    private OpcUaTargetServerSetup(string ServerId,
        string ApplicationName,
        Uri Host,
        ISet<Uri> Endpoints,
        Uri ApplicationNamespace,
        Action<ServerConfiguration>? AdditionalConfiguration)
    {
        this.AdditionalConfiguration = AdditionalConfiguration ?? (_ =>
        {
            /*noop*/
        });
        this.Endpoints = Endpoints;
        OpcUri = new UriBuilder(Host)
        {
            Scheme = "opc.tcp",
            Port = Host.Port // Ensure the port remains unchanged
        }.Uri;
        this.ServerId = ServerId;
        this.ApplicationName = ApplicationName;
        this.Host = Host;
        this.ApplicationNamespace = ApplicationNamespace;
    }

    public Action<ServerConfiguration> AdditionalConfiguration { get; }

    public ISet<Uri> Endpoints { get; }

    public Uri OpcUri { get; }


    public string ServerId { get; }
    public string ApplicationName { get; }
    public Uri Host { get; }
    public Uri ApplicationNamespace { get; }

    public void Deconstruct(out string ServerId, out string ApplicationName, out Uri Host, out ISet<Uri> Endpoints,
        out Uri ApplicationNamespace, out Action<ServerConfiguration>? AdditionalConfiguration)
    {
        ServerId = this.ServerId;
        ApplicationName = this.ApplicationName;
        Host = this.Host;
        Endpoints = this.Endpoints;
        ApplicationNamespace = this.ApplicationNamespace;
        AdditionalConfiguration = this.AdditionalConfiguration;
    }
}