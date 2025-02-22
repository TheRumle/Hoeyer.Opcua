using System;
using Hoeyer.OpcUa.Configuration.EntityServerBuilder;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Configuration;

public static class ServiceExtensions
{
    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services, Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke(EntityServerConfigurationBuilder.Create());
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }
    
    public static OnGoingOpcEntityServiceRegistration AddOpcUaServerConfiguration(this IServiceCollection services, Func<IOpcUaEntityServerInfo> configurationBuilder)
    {
        var entityServerConfiguration = configurationBuilder.Invoke();
        services.AddSingleton(entityServerConfiguration);
        return new OnGoingOpcEntityServiceRegistration(services);
    }   
    
    
}