using System;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

public sealed record OnGoingOpcAgentServiceRegistrationWithSimulation(
    IServiceCollection Collection,
    SimulationServicesConfig Config) : OnGoingOpcAgentServiceRegistration(Collection)
{
    public SimulationServicesContainer SimulationServices { get; } = Config.SimulationServices;
    public SimulationServicesConfig Config { get; } = Config;

    public OnGoingOpcAgentServiceRegistrationWithSimulation CreateReconfigured(
        Action<SimulationServicesConfig> reconfigure)
    {
        reconfigure(new SimulationServicesConfig(Config));
        return new OnGoingOpcAgentServiceRegistrationWithSimulation(Collection, Config);
    }
}