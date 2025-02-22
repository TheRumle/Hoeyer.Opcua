
using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public sealed record OpcUaEntityServerSetup(string ServerId, string ServerName, Uri Host, ISet<Uri> Endpoints, Uri ApplicationNamespace, Action<ServerConfiguration>? AdditionalConfiguration) : IOpcUaEntityServerConfiguration
{

    public Action<ServerConfiguration> AdditionalConfiguration { get; } = AdditionalConfiguration ?? (_ =>
    {
        /*noop*/
    });
    
    
    public string ServerId { get; } = ServerId;
    public string ServerName { get; } = ServerName;
    public Uri Host { get; } = Host;
    public ISet<Uri> Endpoints { get; } = Endpoints;
    public Uri ApplicationNamespace { get; } = ApplicationNamespace;

    public OpcUaEntityServerSetup(IOpcUaEntityServerConfiguration root, Action<ServerConfiguration> additionalConfiguration)
        : this(root.ServerId, root.ServerName, root.Host, root.Endpoints, root.ApplicationNamespace,
            additionalConfiguration)
    {
        
    }


}