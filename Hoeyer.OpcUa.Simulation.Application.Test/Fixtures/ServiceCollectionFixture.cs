using System;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Simulation.Application.Test.Fixtures;

public sealed class ServiceCollectionFixture
{
    public readonly OnGoingOpcEntityServiceRegistrationWithSimulation OngoingRegistration;
    public readonly IServiceProvider ServiceProvider;

    public readonly IServiceCollection SimulationServices;

    public ServiceCollectionFixture()
    {
        OngoingRegistration = new ServiceCollection()
            .WithOpcUaSimulationServices(c => c.WithTimeScaling(float.Epsilon));
        SimulationServices = OngoingRegistration.SimulationServices;
        ServiceProvider = SimulationServices.BuildServiceProvider();
    }
}