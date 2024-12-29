using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        if (opcUaApplicationOptions == null) throw new OptionsNotConfiguredException(typeof(OpcUaServerApplicationOptions));
        
        AddEntityNodeServerServices(services);
    }

    private static void AddEntityNodeServerServices(IServiceCollection services)
    {
        services.AddSingleton<OpcUaEntityServerFactory>();
        services.AddSingleton<ApplicationConfigurationFactory>();
        services.AddSingleton<IApplicationConfigurationFactory>((serviceProvider) => serviceProvider.GetService<ApplicationConfigurationFactory>()!);
                        
        services.AddSingleton<EntityNodeManagerFactory>();
        services.AddSingleton<IEntityNodeManagerFactory>(serviceProvider => serviceProvider.GetService<EntityNodeManagerFactory>()!);
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

internal class OptionsNotConfiguredException : Exception
{
    public OptionsNotConfiguredException(Type optionValueType):base($"An option was not registered before trying to call {nameof(ServiceExtensions.AddOpcUaEntityServerServices)}. The Options pattern must be used to configure an {optionValueType.FullName} as an option, as it is used to configure an OpcUaEntityServer and related services. \n\n You can read more about the options pattern at https://learn.microsoft.com/en-us/dotnet/core/extensions/options")
    {
    }
}

public static class ServiceExtensions
{

    public static IServiceCollection AddOpcUaEntityServerServices(this IServiceCollection services)
    {
        var entities = AppDomain.CurrentDomain.GetAssemblies()
            .AsParallel()
            .SelectMany(assembly => assembly.GetTypes().AsParallel()
                .Where(t => t.IsClass && !t.IsAbstract && Attribute.IsDefined(t, typeof(OpcUaEntityAttribute))));
        
        ServerServicesRegistry.ConfigureServices(entities, services);
        return services;
    }
    
}