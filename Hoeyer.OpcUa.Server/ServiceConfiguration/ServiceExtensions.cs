using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServer(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        serviceRegistration.Collection.AddSingleton<OpcUaEntityServerSetup>(p =>
        {
            var standardConfig = p.GetService<IOpcUaEntityServerInfo>()!;
            if (standardConfig == null)
                throw new InvalidOperationException(
                    $"No {nameof(IOpcUaEntityServerInfo)} has been registered! This should be prevented using builder pattern! SHOULD NOT HAPPEN!");
            return new OpcUaEntityServerSetup(standardConfig, additionalConfiguration ?? (value => { }));
        });

        serviceRegistration.Collection.AddSingleton(p =>
        {
            var loggerFactory = p.GetService<ILoggerFactory>()!;
            var configuration = p.GetService<OpcUaEntityServerSetup>();
            var factories = p.GetService<IEnumerable<IEntityNodeFactory>>() ?? [];
            if (configuration is null)
                throw new InvalidOperationException($"No {nameof(IOpcUaEntityServerInfo)} has been configured!");

            return new OpcUaEntityServerFactory(configuration, factories, loggerFactory);
        });
        return new OnGoingOpcEntityServerServiceRegistration(serviceRegistration.Collection);
    }

    public static OnGoingOpcEntityServerServiceRegistration WithOpcUaServerAsBackgroundService(
        this OnGoingOpcEntityServiceRegistration serviceRegistration,
        Action<ServerConfiguration>? additionalConfiguration = null)
    {
        var serverConfig = serviceRegistration.WithOpcUaServer(additionalConfiguration);
        serverConfig.Collection.AddHostedService<OpcUaServerBackgroundService>();
        return serverConfig;
    }
}