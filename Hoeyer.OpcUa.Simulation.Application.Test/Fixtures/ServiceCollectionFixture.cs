using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Simulation.Application.Test.Fixtures;

public sealed class ServiceCollectionFixture
{
    public readonly OnGoingOpcAgentServiceRegistrationWithSimulation ongoingConfiguration;

    public readonly IServiceCollection SimulationServices;

    public ServiceCollectionFixture()
    {
        ongoingConfiguration = new ServiceCollection()
            .WithAgentServices()
            .WithOpcUaSimulationServices(c => c.WithTimeScaling(float.Epsilon));
        SimulationServices = ongoingConfiguration.SimulationServices;
    }
}