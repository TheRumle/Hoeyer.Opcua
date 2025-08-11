using System;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Client.Application.Browsing.Reading;
using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class ClientServices
{
    public static OnGoingOpcAgentServiceRegistration WithOpcUaClientServices(
        this OnGoingOpcAgentServiceRegistration registration)
    {
        IServiceCollection services = registration.Collection;
        services.AddTransient<BreadthFirstStrategy, BreadthFirstStrategy>();
        services.AddTransient<DepthFirstStrategy, DepthFirstStrategy>();
        services.AddTransient<INodeTreeTraverser, BreadthFirstStrategy>(); //Default strategy
        services.AddTransient<INodeBrowser, NodeBrowser>();
        services.AddTransient<INodeReader, NodeReader>();
        services.AddTransient<IAgentSessionFactory, ReusableSessionFactory>();
        services.AddTransient<ISubscriptionTransferStrategy, CopySubscriptionTransferStrategy>();
        services.AddTransient<IReconnectionStrategy, DefaultReconnectStrategy>();
        services.AddSingleton<AgentMonitoringConfiguration>();

        Type genericMatcher = typeof(AgentDescriptionMatcher<>);
        foreach (Type? m in OpcUaAgentTypes.Entities)
        {
            Type instantiatedMatcher = genericMatcher.MakeGenericType(m);
            services.AddTransient(instantiatedMatcher, _ => DefaultMatcherFactory.CreateMatcher(m));
        }

        return registration;
    }
}