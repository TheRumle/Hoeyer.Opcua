using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Client.Configuration.Entities;
using Hoeyer.OpcUa.Client.Configuration.Entities.Builder;
using Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;
using Hoeyer.OpcUa.Client.Reflection.Configurations;
using Hoeyer.OpcUa.Client.Reflection.Types;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity.State;
using Hoeyer.OpcUa.Observation;
using Hoeyer.OpcUa.Proxy;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Client.Services;

public static class OpcUaServiceExtensions
{
    private static IReadOnlyList<EntityConfigurationContext> GetConfigurators(IEnumerable<Type> types)
    {
        var configurationContexts = types.GetEntityConfigContexts().ToList();
        ValidateContexts(configurationContexts);
        return configurationContexts;
    }

    private static void ValidateContexts(List<EntityConfigurationContext> configurationContexts)
    {
        OpcuaConfigurationException[] invalidContextExceptions = configurationContexts
            .Where(context => !context.EntityType.HasEmptyConstructor())
            .Select(context =>
                new OpcuaConfigurationException(
                    $"{context.EntityType.Name} must have an empty constructor to be configured."))
            .ToArray();

        if (invalidContextExceptions.Any()) throw OpcuaConfigurationException.Merge(invalidContextExceptions);
    }

    public static OnGoingOpcEntityServiceRegistration AddOpcUaClientServices(this OnGoingOpcEntityServiceRegistration services)
    {
        var collection = services.Collection;
        
        //Find and add all configurators to service collection and
        //build a service provider to resolve configurator dependencies s.t they can be used.
        var configurators = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => GetConfigurators(assembly.GetTypes()));

        foreach (var conf in configurators)
        {
            //Add IOpcEntityConfigurator<TEntity>
            collection.AddSingleton(conf.EntityConfiguratorImpl, conf.EntityConfiguratorImpl);
            collection.AddSingleton(conf.EntityConfiguratorInterface, conf.EntityConfiguratorImpl);
            ConfigureServicesFor(conf, collection);
        }

        return services;
    }

     internal static void ConfigureServicesFor(EntityConfigurationContext configuration, IServiceCollection collection)
    {
        var configureServiceMethod = typeof(OpcUaServiceExtensions).GetMethod(nameof(RunConfigurationSetup),
            BindingFlags.NonPublic | BindingFlags.Static)!;
        
        var genericMethod = configureServiceMethod.MakeGenericMethod(configuration.EntityType);
        genericMethod.Invoke(null, [collection]);
    }
     
    internal static void RunConfigurationSetup<TEntity>(IServiceCollection collection) where TEntity : new()
    {
        try
        {
            collection.AddSingleton<EntityConfiguration<TEntity>>(p =>
                {
                    var builder = new EntityConfigurationBuilder<TEntity>();
                    var configurator = p.GetService<IOpcEntityConfigurator<TEntity>>()!;
                    configurator.Configure(builder);
                    return builder.EntityConfiguration;
                })
                .AddSingleton(new StateContainer<TEntity>(new TEntity()))
                .AddTransient<DataValuePropertyAssigner<TEntity>>()
                .AddTransient<OpcUaEntityReader<TEntity>>()
                .AddTransient<IOpcUaNodeConnectionHolder<TEntity>, OpcUaEntityReader<TEntity>>()
                .AddSingleton<SessionFactory>(p =>
                {
                    var opcUaEntityServerConfiguration = p.GetService<IOpcUaEntityServerConfiguration>()!;
                    if (opcUaEntityServerConfiguration == null) throw new InvalidOperationException($"{nameof(IOpcUaEntityServerConfiguration)} has not been configured!");
                    return new SessionFactory(opcUaEntityServerConfiguration);
                })
                .AddTransient<SessionManager>()
                .AddTransient<ISessionManager, SessionManager>()
                .AddSingleton(typeof(IEntityObserver<TEntity>), typeof(EntityProxy<TEntity>))
                .AddSingleton<EntityProxy<TEntity>>()
                .AddSingleton<SubscriptionEngine<TEntity>>()
                .AddSingleton<Func<IStateChangeSubscriber<TEntity>, StateChangeSubscription<TEntity>>>(
                    serviceProvider => serviceProvider.GetService<SubscriptionEngine<TEntity>>()!.SubscribeToMachine);
        }
        catch (InvalidOperationException e)
        {
            throw new OpcuaConfigurationException(
                $"Could not setup configuration of IOpcEntityConfigurator<{typeof(TEntity).Name}>. If the implementor depends on other services these must be registered before ${nameof(OpcUaServiceExtensions.AddOpcUaClientServices)} is called. \n\n" +
                e);
        }
    }
}