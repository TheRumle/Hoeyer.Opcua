﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Opc.Ua;

namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

internal class EntityServerConfigurationBuilder : IEntityServerConfigurationBuilder, IServerNameStep, IHostStep, IEndpointsStep, IAdditionalConfigurationStep
{
    private string _serverId = string.Empty;
    private string _serverName = string.Empty;
    private string _host = string.Empty;
    private List<string> _endpoints = new();
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

    public IEndpointsStep WithHttpHost(string host)
    {
        _host = "http://" + host;
        return this;
    }

    /// <inheritdoc />
    public IEndpointsStep WithOpcTcpHost(string host)
    {
        _host ="opc.tcp://" + host;
        return this;
    }

    public IEntityServerConfigurationBuildable WithEndpoints(List<string> endpoints)
    {
        _endpoints = endpoints;
        return this;
    }

    public OpcUaEntityServerConfiguration Build()
    {
        var validuri = Uri.TryCreate( string.Format(CultureInfo.InvariantCulture, "{0}", _host), UriKind.RelativeOrAbsolute, out var uri);
        if (!validuri) throw new ArgumentException($"Host and serverId could not form a valid URI: {uri}");

        return new OpcUaEntityServerConfiguration(_serverId, _serverName, _host, _endpoints, uri);
    }

    /// <inheritdoc />
    public IEntityServerConfigurationBuildable WithAdditionalConfiguration(Action<ServerConfiguration> additionalConfigurations)
    {
        return this;
    }
}