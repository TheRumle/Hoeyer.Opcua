using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Playground.Modelling.Models;
using Playground.Server;

namespace Simulation.Application.Test.Fixtures;

public sealed class ServiceCollectionFixture
{
    public readonly OnGoingOpcEntityServiceRegistrationWithSimulation ongoingConfiguration;

    public readonly IServiceCollection SimulationServices;

    public ServiceCollectionFixture()
    {
        ongoingConfiguration = new ServiceCollection()
            .AddOpcUa(e =>
                e.WithServerId("id").WithServerName("name").WithWebOrigins(WebProtocol.OpcTcp, "localhost", 1111)
                    .Build())
            .WithEntityModelsFrom(typeof(Gantry))
            .WithOpcUaSimulationServices(c => { c.WithTimeScaling(float.Epsilon); },
                typeof(GantryLoader));
        SimulationServices = ongoingConfiguration.Services;
    }
}