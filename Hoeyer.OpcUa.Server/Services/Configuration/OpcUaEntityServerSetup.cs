﻿using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Services.Configuration;

public sealed record OpcUaEntityServerSetup(
    string ServerId,
    string ApplicationName,
    Uri Host,
    ISet<Uri> Endpoints,
    Uri ApplicationNamespace,
    Action<ServerConfiguration>? AdditionalConfiguration) : IOpcUaEntityServerInfo
{
    public OpcUaEntityServerSetup(IOpcUaEntityServerInfo root, Action<ServerConfiguration> additionalConfiguration)
        : this(root.ServerId, root.ApplicationName, root.Host, root.Endpoints, root.ApplicationNamespace,
            additionalConfiguration)
    {
    }

    public Action<ServerConfiguration> AdditionalConfiguration { get; } = AdditionalConfiguration ?? (_ =>
    {
        /*noop*/
    });


    public string ServerId { get; } = ServerId;
    public string ApplicationName { get; } = ApplicationName;
    public Uri Host { get; } = Host;
    public ISet<Uri> Endpoints { get; } = Endpoints;
    public Uri ApplicationNamespace { get; } = ApplicationNamespace;

    /// <inheritdoc />
    public Uri OpcUri { get; } = new UriBuilder(Host)
    {
        Scheme = "opc.tcp",
        Port = Host.Port // Ensure the port remains unchanged
    }.Uri;
}