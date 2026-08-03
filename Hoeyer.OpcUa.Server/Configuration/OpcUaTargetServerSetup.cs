using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Abstractions;

namespace Hoeyer.OpcUa.Server.Configuration;

public sealed record OpcUaTargetServerSetup : IOpcUaTargetServerSetup
{
    public OpcUaTargetServerSetup(IApplicationConfigurationRequirements root)
        : this(root.ServerId, root.ApplicationName, root.Host, new HashSet<Uri>([root.Host]), root.ApplicationNamespace)
    {
    }

    private OpcUaTargetServerSetup(string ServerId,
        string ApplicationName,
        Uri Host,
        ISet<Uri> Endpoints,
        Uri ApplicationNamespace)
    {
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

    public Uri OpcUri { get; }


    public string ServerId { get; }
    public Uri Host { get; }


    public ISet<Uri> Endpoints { get; }
    public string ApplicationName { get; }
    public Uri ApplicationNamespace { get; }
}