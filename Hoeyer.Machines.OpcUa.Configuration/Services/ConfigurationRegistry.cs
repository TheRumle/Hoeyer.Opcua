using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Configurations;
using Hoeyer.Machines.OpcUa.Configuration.Reflection.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoeyer.Machines.OpcUa.Configuration.Services;

/// <summary>
/// Uses the <paramref name="services"/> to create a <see cref="ServiceProvider"/> and uses it to wire up services necessary for configuring OpcUa entities. Disposing the <see cref="ConfigurationRegistry"/> will remove services used for internal wiring from <paramref name="services"/>.
/// </summary>
/// <param name="services">The collection providing the register with services necessary for the creation of <see cref="EntityOpcUaMapping{T}"/>. </param>
internal sealed class ConfigurationRegistry(IServiceCollection services) : IDisposable
{
    private List<EntityConfigurationContext> _configurators = [];

    /// <summary>
    /// Add nodes and configurator implementations to service collection.
    /// </summary>
    private void AddContextTypes()
    {
        foreach (var conf in _configurators)
        {
            services.AddSingleton(conf.EntityType, conf.EntityType.CreateUninitalizedInstance());
            services.AddScoped(conf.ConcreteConfiguratorInterface, conf.ImplementorType);
            services.AddScoped(conf.ImplementorType, conf.ImplementorType);
        }
    }
    
    /// <summary>
    /// Remove configurator services, both interfaces and concrete implementation but does not remove the Entity node!
    /// </summary>
    private void RemoveConfigurationServices() {
        foreach (var configurator in _configurators)
        {
            services.RemoveAll(configurator.ConcreteConfiguratorInterface);
            services.RemoveAll(configurator.ImplementorType);
        }
        _configurators.Clear();
    }



    internal void ConfigureServices(IEnumerable<EntityConfigurationContext> configurators)
    {
        
        _configurators = configurators.ToList();
        AddContextTypes();
        var scope = services.BuildServiceProvider().CreateScope();
        foreach (EntityConfigurationContext configurator in _configurators)
        {
            Type entityType = configurator.EntityType;
            
            // Use reflection to invoke ConfigureService<TEntity>() with entityType
            MethodInfo configureServiceMethod = GetType().GetMethod(nameof(ConfigureService))!;
            MethodInfo genericMethod = configureServiceMethod.MakeGenericMethod(entityType);
            // Invoke the generic method
            genericMethod.Invoke(this, [configurator, scope]);
            
        }
    }

    private void ConfigureService<TEntity>(EntityConfigurationContext configurator, IServiceScope scope) where TEntity : new()
    {
        var configuratorInstance = scope.ServiceProvider.GetService(configurator.ImplementorType);
        EntityConfiguration<TEntity> entityConfiguration = configurator.InvokeConfigurationMethod<TEntity>(configuratorInstance);

        services.AddSingleton(typeof(EntityConfiguration<TEntity>), entityConfiguration);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        RemoveConfigurationServices();
    }
}