using System;
using Hoeyer.OpcUa.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

public sealed record OnGoingOpcEntityServiceRegistrationWithSimulation(
    IServiceCollection Collection,
    SimulationServicesConfig Config) : OnGoingOpcEntityServiceRegistration(Collection)
{
    public SimulationServicesContainer Services { get; } = Config.SimulationServices;
    public SimulationServicesConfig Config { get; } = Config;

    public OnGoingOpcEntityServiceRegistrationWithSimulation CreateReconfigured(
        Action<SimulationServicesConfig> reconfigure)
    {
        reconfigure(new SimulationServicesConfig(Config));
        return new OnGoingOpcEntityServiceRegistrationWithSimulation(Collection, Config);
    }
}