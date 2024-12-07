using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

public static class OpcUaServiceExtensions
{
    public static IServiceCollection AddOpcUaConfiguration(this IServiceCollection services)
    {
        var configurators = Assembly.GetExecutingAssembly().OpcUaConfigurators();
        foreach (var configurator in configurators)
        {
            var genericInterface = configurator.GenericInterfaceType;
            services.AddTransient(genericInterface, configurator.ImplementorType);
            services.AddTransient(configurator.ImplementorType);

            var configurationBeginning = configurator.NodeSectionSelectionStepType;
            services.AddTransient(configurationBeginning, configurationBeginning);
        }

        foreach (OpcUaNodeConfigurationType configurator in configurators)
        {
            configurator.InvokeConfigureMethod();
            OpcUaNodeSetupContext context = configurator.NodeSelectionTypeInstance.Context;
            
            
            //Look through configurations for this context. 
        }
        
        

        return services;
    }
}