using System;
using System.Collections.Generic;
using System.Globalization;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public sealed record EntityServerConfiguration(string ServerId, string ServerName, string Host, List<string> Endpoints,
    Uri ApplicationNamespace, Action<ServerConfiguration>? AdditionalConfiguration = null)
{
    public string ServerId { get; } = ServerId;
    public string ServerName { get; } = ServerName;
    public string Host { get; } = Host;
    public List<string> Endpoints { get; set; } = Endpoints;

    /// <summary>
    /// For instance, http://samples.org/UA/MyApplication or something else uniqely identifying the overall resource,
    /// </summary>
    public Uri ApplicationNamespace { get; } = ApplicationNamespace;
    public Action<ServerConfiguration> AdditionalConfiguration { get; } = AdditionalConfiguration ?? (_ => { /*noop*/});
}