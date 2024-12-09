using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

public static class OpcUaServiceExtensions
{
    private static IReadOnlyList<OpcUaNodeConfiguratorContext> AddConfigurators(this IServiceCollection serviceCollection, IEnumerable<Type> types)
    {   
        var configurators = types.GetOpcUaConfigurators().ToList();
        foreach (var conf in configurators)
        {
            serviceCollection.AddScoped(conf.ConcreteConfiguratorInterface, conf.ImplementorType);
            serviceCollection.AddScoped( conf.ImplementorType, conf.ImplementorType);
        }
        
        return configurators;
    }
    
    public static IServiceCollection AddOpcUa(this IServiceCollection services)
    {
        //Find and add all configurators to service collection and
        //build a service provider to resolve configurator dependencies s.t they can be used.
        var configurators = services.AddConfigurators(Assembly.GetCallingAssembly().GetTypes());
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateScope();
        foreach (OpcUaNodeConfiguratorContext configurator in configurators)
        {
            var configuratorInstance = scope.ServiceProvider.GetService(configurator.ImplementorType);
            var setupContext = configurator.InvokeConfigurationMethod(configuratorInstance);
            

        }
        

        
        
        
    
        

        return services;
    }
}