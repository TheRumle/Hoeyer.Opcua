using System;
using System.Collections.Generic;
using System.Globalization;
using Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

internal class EntityServerConfigurationBuilder : IEntityServerConfigurationBuilder, IServerNameStep, IHostStep, IEndpointsStep, IAdditionalConfigurationStep
{
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;
    private string _host = string.Empty;
    private List<string> _endpoints = new();
    private Action<ServerConfiguration>? additionalConfiguration;

    private EntityServerConfigurationBuilder()
    {
    }

    public static IEntityServerConfigurationBuilder Create() => new EntityServerConfigurationBuilder();

    public IServerNameStep WithServerId(string serverId)
    {
        _serverId = serverId;
        return this;
    }

    public IHostStep WithServerName(string serverName)
    {
        _serverName = serverName;
        return this;
    }

    public IEndpointsStep WithHost(string host)
    {
        _host = host;
        return this;
    }

    public IEntityServerConfigurationBuildable WithEndpoints(List<string> endpoints)
    {
        _endpoints = endpoints;
        return this;
    }

    public EntityServerConfiguration Build()
    {
        var validUrn = Uri.TryCreate( string.Format(CultureInfo.InvariantCulture, "urn:{0}.{1}", _host, _serverId), UriKind.Absolute, out Uri uri);
        if (!validUrn) throw new ArgumentException($"Host and serverId could not form a valid URN: {uri}");

        return new EntityServerConfiguration(_serverId, _serverName, _host, _endpoints, uri, additionalConfiguration);
    }

    /// <inheritdoc />
    public IEntityServerConfigurationBuildable WithAdditionalConfiguration(Action<ServerConfiguration> additionalConfigurations)
    {
        this.additionalConfiguration = additionalConfigurations;
        return this;
    }
}