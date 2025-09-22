using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EntityModelling.Models;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Simulation.Application.Test.Fixtures;

public sealed class ServiceCollectionFixture
{
    public readonly OnGoingOpcEntityServiceRegistrationWithSimulation ongoingConfiguration;

    public readonly IServiceCollection SimulationServices;

    public ServiceCollectionFixture()
    {
        ongoingConfiguration = new ServiceCollection()
            .WithEntityServices()
            .WithOpcUaSimulationServices(c => { c.WithTimeScaling(float.Epsilon); },
                typeof(Gantry).Assembly);
        SimulationServices = ongoingConfiguration.SimulationServices;
    }
}