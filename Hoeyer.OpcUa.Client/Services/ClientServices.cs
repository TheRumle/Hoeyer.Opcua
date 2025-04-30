using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Api.Reading;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Monitoring;
using Hoeyer.OpcUa.Client.Application.Reading;
using Hoeyer.OpcUa.Client.MachineProxy;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class ClientServices
{
    public static OnGoingOpcEntityServiceRegistration WithOpcUaClientServices(
        this OnGoingOpcEntityServiceRegistration registration)
    {
        var services = registration.Collection;
        services.AddTransient<BreadthFirstStrategy, BreadthFirstStrategy>();
        services.AddTransient<DepthFirstStrategy, DepthFirstStrategy>();
        services.AddTransient<INodeTreeTraverser, BreadthFirstStrategy>(); //Default strategy
        services.AddTransient<INodeBrowser, NodeBrowser>();
        services.AddTransient<INodeReader, NodeReader>();
        services.AddTransient<IEntitySessionFactory, SessionFactory>();
        services.AddSingleton<EntityMonitoringConfiguration>();

        var genericMatcher = typeof(EntityDescriptionMatcher<>);
        foreach (var m in OpcUaEntityTypes.Entities)
        {
            var instantiatedMatcher = genericMatcher.MakeGenericType(m);
            services.AddTransient(instantiatedMatcher, _ => DefaultMatcherFactory.CreateMatcher(m));
        }
        return registration;
    }

}