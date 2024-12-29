using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Services;

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