using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Architecture;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging.Subscriptions;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

/// <summary>
///     Provides an interface that can be used to adapt the simulation services. The methods of this class provides an
///     <see cref="IServiceProvider" /> that previously added services as well as simulation specific services that has
///     been added only for this purpose.
/// </summary>
public class SimulationServicesConfig
{
    private readonly IServiceCollection _originalServices;
    public readonly IReadOnlyList<(Type entityType, Type methodArgs)> ActionSimulationPatterns;
    public readonly IReadOnlyList<(Type entityType, Type methodArgs, Type returnType)> FunctionSimulationPatterns;
    public readonly SimulationServicesContainer SimulationServices;

    public SimulationServicesConfig(SimulationServicesConfig source)
    {
        _originalServices = source._originalServices;
        SimulationServices = source.SimulationServices;
        ActionSimulationPatterns = source.ActionSimulationPatterns.ToList();
        FunctionSimulationPatterns = source.FunctionSimulationPatterns.ToList();
        TimeScaler = source.TimeScaler;
    }

    public SimulationServicesConfig(IServiceCollection originalServices,
        SimulationServicesContainer simulationServices,
        List<SimulationPatternTypeDetails> actionSimulationPatterns,
        List<SimulationPatternTypeDetails> functionSimulationPatterns)
    {
        SimulationServices = simulationServices;
        _originalServices = originalServices;
        ActionSimulationPatterns = actionSimulationPatterns.Select(e => (e.Entity, e.MethodArgType)).ToList();
        FunctionSimulationPatterns = functionSimulationPatterns
            .Select(e => (e.Entity, e.MethodArgType, e.UnwrappedReturnType!)).ToList();

        originalServices.AddTransient<ITimeScaler, IdentityTimeScaler>();
        SimulationServices.AddTransient<ITimeScaler, IdentityTimeScaler>();
    }

    internal ITimeScaler TimeScaler { get; set; } = new IdentityTimeScaler();

    public SimulationServicesConfig WithTimeScaling(double scalingFactor)
    {
        SimulationServices.AddTransient<ITimeScaler>(p => new TimeScaler(scalingFactor));
        return this;
    }

    public SimulationServicesConfig WithTimeScaling(Func<IServiceProvider, ITimeScaler> scaler)
    {
        SimulationServices.AddTransient(scaler);
        return this;
    }

    public SimulationServicesConfig WithTimeScaling(Func<ITimeScaler> scalerProvider)
    {
        SimulationServices.AddTransient<ITimeScaler>(p => scalerProvider.Invoke());
        return this;
    }

    public SimulationServicesConfig WithTimeScaling(ITimeScaler scalerProvider)
    {
        SimulationServices.AddTransient<ITimeScaler>(p => scalerProvider);
        return this;
    }

    public SimulationServicesConfig WithSubscriptionFactory(Type factoryType)
    {
        var factoryInterface = typeof(IMessageSubscriptionFactory<>);
        var wantedFactory = factoryType.GetInterfaces().FirstOrDefault(e =>
            e.IsConstructedGenericType && e.GetGenericTypeDefinition() == factoryInterface);

        if (wantedFactory is null)
        {
            throw new ArgumentException(nameof(factoryType) + $". The type must implement {factoryInterface.FullName}");
        }

        SimulationServices.AddSingleton(factoryType, factoryInterface);
        return this;
    }

    public SimulationServicesConfig WithSubscriptionFactory<TFactory, TEntity>()
        where TFactory : class, IMessageSubscriptionFactory<SimulationResult<TEntity>>
    {
        SimulationServices.AddSingleton<IMessageSubscriptionFactory<SimulationResult<TEntity>>, TFactory>();
        return this;
    }

    public SimulationServicesConfig AdaptToRuntime<TAdapter>()
        where TAdapter : class, ILayerAdapter<SimulationServicesConfig>
    {
        GetServiceAndThen<TAdapter>(adapter => adapter.Adapt(this, _originalServices));
        return this;
    }

    private void GetServiceAndThen<TService>(Action<TService> then) where TService : class
    {
        var services = new ServiceCollection();
        services.AddRange(SimulationServices);
        services.AddRange(_originalServices);
        services.AddScoped<TService>();
        using var scope = services.BuildServiceProvider().CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        then.Invoke(service);
    }
}