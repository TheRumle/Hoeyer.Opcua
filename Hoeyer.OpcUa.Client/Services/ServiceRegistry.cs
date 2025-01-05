using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Observation;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Client.Configuration.Entities.Builder;
using Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;
using Hoeyer.OpcUa.Client.Reflection.Configurations;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity.State;
using Hoeyer.OpcUa.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Client.Services;


public sealed class ReversibleCollection(IServiceCollection collection) : IDisposable
{
    public List<Type> RegisteredTypes { get; } = new();
    public void AddSingleTon(Type t)
    {
        collection.AddSingleton(t);
        RegisteredTypes.Add(t);
    }

    public void AddTransient(Type service, Type impl)
    {
        collection.AddTransient(service, impl);
        RegisteredTypes.Add(service);
        RegisteredTypes.Add(impl);
    }

    public void Reverse()
    {
        foreach (var type in RegisteredTypes)
        {
            collection.RemoveAll(type);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Reverse();
    }
}

/// <summary>
/// Uses the <paramref name="services"/> to create a <see cref="ServiceProvider"/> and uses it to wire up services necessary for configuring OpcUa entities. Disposing the <see cref="ServiceRegistry"/> will remove services used for internal wiring from <paramref name="services"/>.
/// </summary>
/// <param name="services">The collection providing the register with services necessary for the creation of a number of services related to OpcUa state observation</param>
internal sealed class ServiceRegistry(IServiceCollection services) : IDisposable
{
    private List<EntityConfigurationContext> _configurators = [];
    private ReversibleCollection _reversibleCollection = new(services);


    internal void ConfigureServices(IEnumerable<EntityConfigurationContext> configurators)
    {
        
        _configurators = configurators.ToList();
        foreach (var conf in _configurators)
        {
            _reversibleCollection.AddTransient(conf.ImplementorType, conf.ImplementorType);
            _reversibleCollection.AddTransient(conf.ConcreteConfiguratorInterface, conf.ImplementorType);
        }
        
        var scope = services.BuildServiceProvider().CreateScope();
        
        foreach (EntityConfigurationContext conf in _configurators)
        {

            // Use reflection to invoke this.RunConfigurationSetup<TEntity>() with entityType. This is done to get better IDE within the invoked method. 
            MethodInfo configureServiceMethod = GetType().GetMethod(nameof(RunConfigurationSetup), BindingFlags.NonPublic | BindingFlags.Instance)!;
            MethodInfo genericMethod = configureServiceMethod.MakeGenericMethod(conf.EntityType);

            genericMethod.Invoke(this, [scope]);
        }
    }



    /// <summary>
    /// Runs the configuratior 
    /// </summary>
    /// <param name="scope"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <exception cref="OpcuaConfigurationException"></exception>
    private void RunConfigurationSetup<TEntity>(IServiceScope scope) where TEntity : new()
    {
        try
        {
            var configuratorInstance = scope.ServiceProvider.GetService<IOpcEntityConfigurator<TEntity>>();
            if (configuratorInstance == null) throw new OpcuaConfigurationException($"Could not find implementation of IOpcEntityConfigurator<{typeof(TEntity).Name}>");
            
            var configurationBuilder = new EntityConfigurationBuilder<TEntity>();
            configuratorInstance.Configure(configurationBuilder);
            
            var opcUaApplicationOptions = scope.ServiceProvider.GetService<IOptions<OpcUaApplicationOptions>>();
            if (opcUaApplicationOptions == null) throw new OptionsNotConfiguredException(typeof(OpcUaApplicationOptions), Assembly.GetExecutingAssembly());

            services.AddSingleton(configurationBuilder.EntityConfiguration)
                .AddSingleton(new StateContainer<TEntity>(new TEntity()))
                .AddTransient<DataValuePropertyAssigner<TEntity>>()
                .AddTransient<OpcUaEntityReader<TEntity>>()
                .AddTransient<IOpcUaNodeConnectionHolder<TEntity>, OpcUaEntityReader<TEntity>>()
                .AddSingleton(new SessionFactory(opcUaApplicationOptions.Value))
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
            throw new OpcuaConfigurationException($"Could not setup configuration of  IOpcEntityConfigurator<{typeof(TEntity).Name}>. If the implementor depends on other services these must be registered before ${nameof(OpcUaServiceExtensions.AddOpcUaClientServices)} is called. \n\n" + e );
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _reversibleCollection.Reverse();
    }
}