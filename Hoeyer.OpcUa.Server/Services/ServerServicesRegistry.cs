using System;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Nodes;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Server.Services;

internal static class ServerServicesRegistry
{
    internal static void ConfigureServices(IServiceCollection services)
    {
        var entityCreator = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEntityNodeCreator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEntityNodeCreator>();

        // Register them as a singleton IEnumerable<IEntityNodeCreator>
        services.AddSingleton(entityCreator);
        AddEntityNodeServerServices(services);
    }

    private static void AddEntityNodeServerServices(IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var opcUaApplicationOptions = serviceProvider.GetService<IOptions<OpcUaApplicationOptions>>();
        //Check if options are configured s.t the server factory can be created
        if (opcUaApplicationOptions == null) throw new OptionsNotConfiguredException(typeof(OpcUaApplicationOptions), Assembly.GetExecutingAssembly());
        
        services.AddSingleton<OpcUaEntityServerFactory>();
        services.AddSingleton<ApplicationConfigurationFactory>();
        services.AddSingleton<IApplicationConfigurationFactory>((serviceProvider) => serviceProvider.GetService<ApplicationConfigurationFactory>()!);
    }
}