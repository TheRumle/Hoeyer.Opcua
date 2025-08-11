using System;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.AgentServerBuilder;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public static class Services
{
    public static OnGoingOpcAgentServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IAgentServerConfigurationBuilder, IOpcUaAgentServerInfo> configurationBuilder)
    {
        var agentServerConfiguration = configurationBuilder.Invoke(AgentServerConfigurationBuilder.Create());
        services.AddSingleton(agentServerConfiguration);
        return new OnGoingOpcAgentServiceRegistration(services);
    }

    public static IServiceCollection WithAgentServices(this IServiceCollection services)
    {
        var errs = services
            .AddAgentServices()
            .ToList();
        if (errs.Any()) throw new OpcUaAgentServiceConfigurationException(errs);
        return services;
    }

    public static OnGoingOpcAgentServiceRegistration WithAgentServices(
        this OnGoingOpcAgentServiceRegistration registration)
    {
        registration.Collection.WithAgentServices();
        return registration;
    }
}