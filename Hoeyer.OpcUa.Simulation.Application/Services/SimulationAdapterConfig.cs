using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

/// <summary>
/// Provides an interface that can be used to adapt the simulation services. The methods of this class provides an <see cref="IServiceProvider"/> that previously added services as well as simulation specific services that has been added only for this purpose. 
/// </summary>
public class SimulationAdapterConfig
{
    public readonly IReadOnlyList<(Type entityType, Type methodArgs)> ActionSimulationPatterns;
    public readonly IReadOnlyList<(Type entityType, Type methodArgs, Type returnType)> FunctionSimulationPatterns;
    public readonly IServiceCollection Services;
    public readonly SimulationServicesContainer SimulationServicesContainer;

    /// <summary>
    /// Provides an interface that can be used to adapt the simulation services. The methods of this class provides an <see cref="IServiceProvider"/> that previously added services as well as simulation specific services that has been added only for this purpose. 
    /// </summary>
    /// <param name="simulationServicesContainer"></param>
    /// <param name="actionSimulationPatterns"></param>
    /// <param name="functionSimulationPatterns"></param>
    public SimulationAdapterConfig(IServiceCollection originalServices,
        SimulationServicesContainer simulationServicesContainer,
        List<SimulationPatternTypeDetails> actionSimulationPatterns,
        List<SimulationPatternTypeDetails> functionSimulationPatterns)
    {
        SimulationServicesContainer = simulationServicesContainer;
        Services = originalServices;
        ActionSimulationPatterns = actionSimulationPatterns.Select(e => (e.Entity, e.MethodArgType)).ToList();

        FunctionSimulationPatterns = functionSimulationPatterns
            .Select(e => (e.Entity, e.MethodArgType, e.UnwrappedReturnType)).ToList();
        originalServices.AddTransient<ITimeScaler, IdentityTimeScaler>();
        SimulationServicesContainer.AddTransient<ITimeScaler, IdentityTimeScaler>();
    }

    internal ITimeScaler TimeScaler { get; set; } = new IdentityTimeScaler();

    public SimulationAdapterConfig WithTimeScaler(Func<IServiceProvider, ITimeScaler> scaler)
    {
        SimulationServicesContainer.AddTransient(scaler);
        return this;
    }

    public SimulationAdapterConfig WithTimeScaling(double scalingFactor)
    {
        SimulationServicesContainer.AddTransient<ITimeScaler>(p => new TimeScaler(scalingFactor));
        return this;
    }

    public SimulationAdapterConfig WithTimeScaler(Func<ITimeScaler> scalerProvider)
    {
        SimulationServicesContainer.AddTransient<ITimeScaler>(p => scalerProvider.Invoke());
        return this;
    }
}