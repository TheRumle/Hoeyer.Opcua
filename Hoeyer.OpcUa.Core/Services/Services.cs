using System;
using System.Linq;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public static class Services
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services,
        Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }

    public static OnGoingOpcEntityServiceRegistration WithEntityServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var services = registration.Collection;
        var errs = services
            .AddEntityServices()
            .ToList();
        if (errs.Any()) throw new OpcUaEntityServiceConfigurationException(errs);


        return registration;
    }
}