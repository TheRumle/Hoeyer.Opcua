using System;
using System.Collections.Generic;
using System.Globalization;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;

internal class AgentServerConfigurationBuilder : IAgentServerConfigurationBuilder, IServerNameStep, IHostStep,
    IEndpointsStep, IAdditionalConfigurationStep
{
    private readonly HashSet<Uri> _endpoints = new();
    private Uri _host = null!;
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;

    private AgentServerConfigurationBuilder()
    {
    }

    /// <inheritdoc />
    public IAgentServerConfigurationBuildable WithAdditionalConfiguration(
        Action<ServerConfiguration> additionalConfigurations)
    {
        return this;
    }

    public IAgentServerConfigurationBuildable WithEndpoints(List<string> endpoints)
    {
        foreach (var e in endpoints) _endpoints.Add(new Uri(e));
        return this;
    }

    public IOpcUaAgentServerInfo Build()
    {
        var validuri = Uri.TryCreate(string.Format(CultureInfo.InvariantCulture, "{0}", _host),
            UriKind.RelativeOrAbsolute, out var uri);
        if (!validuri)
        {
            throw new ArgumentException($"Host and serverId could not form a valid URI: {uri}");
        }

        return new OpcUaAgentServerInfo(_serverId, _serverName, _host, _endpoints, uri);
    }

    public IServerNameStep WithServerId(string serverId)
    {
        _serverId = serverId;
        return this;
    }

    public IEndpointsStep WithHttpsHost(string host, int port)
    {
        _host = new Uri("https://" + host + ":" + port);
        _endpoints.Add(_host);
        return this;
    }

    /// <inheritdoc />
    public IEndpointsStep WithOpcTcpHost(string host, int port)
    {
        _host = new Uri("opc.tcp://" + host + ":" + port);
        _endpoints.Add(_host);
        return this;
    }

    public IHostStep WithServerName(string serverName)
    {
        _serverName = serverName;
        return this;
    }

    public static IAgentServerConfigurationBuilder Create()
    {
        return new AgentServerConfigurationBuilder();
    }
}