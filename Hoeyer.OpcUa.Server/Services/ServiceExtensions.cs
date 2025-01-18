using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Server.Services;

public static class ServiceExtensions
{
    public static void AddOpcUaEntityServerServices(this IServiceCollection services)
    {
        IEnumerable<IEntityNodeCreator> entityCreator = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEntityNodeCreator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEntityNodeCreator>();

        services.AddSingleton(entityCreator);


        AssertOptionsConfigured(services);
        services.AddSingleton<OpcUaEntityServerFactory>();
        services.AddSingleton<ApplicationConfigurationFactory>();
        services.AddSingleton<IApplicationConfigurationFactory>(serviceProvider =>
            serviceProvider.GetService<ApplicationConfigurationFactory>()!);
    }

    private static void AssertOptionsConfigured(IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var opcUaApplicationOptions = serviceProvider.GetService<IOptions<OpcUaApplicationOptions>>();
        if (opcUaApplicationOptions == null)
            throw new OptionsNotConfiguredException(typeof(OpcUaApplicationOptions), Assembly.GetExecutingAssembly());
    }
}