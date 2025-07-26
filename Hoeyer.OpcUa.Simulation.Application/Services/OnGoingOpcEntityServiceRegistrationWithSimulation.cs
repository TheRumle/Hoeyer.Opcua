using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

public sealed record OnGoingOpcEntityServiceRegistrationWithSimulation(
    IServiceCollection Collection,
    SimulationServicesContainer SimulationServices) : OnGoingOpcEntityServiceRegistration(Collection)
{
    public SimulationServicesContainer SimulationServices { get; } = SimulationServices;
}