using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Configuration;

public interface IOpcUaEntityServerConfiguration
{
    string ServerId { get; }
    string ServerName { get; }
    string Host { get; }
    List<string> Endpoints { get; set; }

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    Uri ApplicationNamespace { get; }
}

public sealed record OpcUaEntityServerConfiguration(string ServerId, string ServerName, string Host, List<string> Endpoints,
    Uri ApplicationNamespace) : IOpcUaEntityServerConfiguration
{
    public string ServerId { get; } = ServerId;
    public string ServerName { get; } = ServerName;
    public string Host { get; } = Host;
    public List<string> Endpoints { get; set; } = Endpoints;

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; } = ApplicationNamespace;
}