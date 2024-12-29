using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Server.Services;

internal static class ServerServicesRegistry
{
    internal static void ConfigureServices(IEnumerable<Type> entityTypes, IServiceCollection services)
    {
        AddEntityNodeCreation(entityTypes, services);
        
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var opcUaApplicationOptions = serviceProvider.GetService<IOptions<OpcUaServerApplicationOptions>>();
        if (opcUaApplicationOptions == null) throw new OptionsNotConfiguredException(typeof(OpcUaServerApplicationOptions), Assembly.GetExecutingAssembly());
        
        AddEntityNodeServerServices(services);
    }

    private static void AddEntityNodeServerServices(IServiceCollection services)
    {
        services.AddSingleton<OpcUaEntityServerFactory>();
        services.AddSingleton<ApplicationConfigurationFactory>();
        services.AddSingleton<IApplicationConfigurationFactory>((serviceProvider) => serviceProvider.GetService<ApplicationConfigurationFactory>()!);
    }

    private static void AddEntityNodeCreation(IEnumerable<Type> entityTypes, IServiceCollection services)
    {
        foreach (var type in entityTypes)
        {

            // Use reflection to invoke this.RunConfigurationSetup<TEntity>() with entityType. This is done to get better IDE within the invoked method. 
            MethodInfo configureServiceMethod = typeof(ServerServicesRegistry)
                .GetMethod(
                    name: nameof(RunConfigurationSetup), 
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)!;
            
            MethodInfo genericMethod = configureServiceMethod.MakeGenericMethod(type);
            genericMethod.Invoke(null, [services]);
        }
    }

    private static void RunConfigurationSetup<TEntity>(IServiceCollection services)
    {
        EntityObjectStateCreator<TEntity> entityObjectStateCreator = new();
        services.AddSingleton(entityObjectStateCreator);
        services.AddSingleton<IEntityObjectStateCreator>(entityObjectStateCreator);
    }
}