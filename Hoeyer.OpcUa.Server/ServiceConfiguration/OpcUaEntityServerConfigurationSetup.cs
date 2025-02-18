
using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Configuration;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public sealed class OpcUaEntityServerConfigurationSetup(OpcUaEntityServerConfiguration entityServerConfiguration, Action<ServerConfiguration> additionalConfiguration = null)
{
    
    public Action<ServerConfiguration> AdditionalConfiguration { get; } = additionalConfiguration ?? (_ => { /*noop*/});
  
    public string ServerId { get; } = entityServerConfiguration.ServerId;
    public string ServerName { get; } = entityServerConfiguration.ServerName;
    public string Host { get; } = entityServerConfiguration.Host;
    public List<string> Endpoints { get; set; } = entityServerConfiguration.Endpoints;

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; } = entityServerConfiguration.ApplicationNamespace;

    public OpcUaEntityServerConfiguration EntityServerConfiguration { get; } = entityServerConfiguration;


    public OpcUaEntityServerConfigurationSetup(string serverId, string serverName, string host, List<string> endpoints, Uri result, Action<ServerConfiguration>? additionalConfiguration) : this(new OpcUaEntityServerConfiguration(serverId, serverName, host, endpoints, result), additionalConfiguration)
    {}
}