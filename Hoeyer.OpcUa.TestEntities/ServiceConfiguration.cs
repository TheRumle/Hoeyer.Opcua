using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.TestEntities;

public static class ServiceConfiguration
{
    public static IServiceCollection AddRunningTestEntityServices(
        this IServiceCollection collection, Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> serverSetup,
        Action<SimulationServicesConfig>? simulationSetup = null)
    {
        var simSetup = simulationSetup ?? (configure =>
        {
            configure.WithTimeScaling(double.Epsilon);
            configure.AdaptToRuntime<ServerSimulationAdapter>();
        });

        collection.AddOpcUaServerConfiguration(serverSetup)
            .WithEntityServices()
            .WithOpcUaClientServices()
            .WithOpcUaSimulationServices(simSetup)
            .WithOpcUaServerAsBackgroundService()
            .Collection.AddLogging(e => e.AddSimpleConsole());
        return collection;
    }

    public static IServiceCollection AddTestEntityServices(
        this IServiceCollection collection, Func<IEntityServerConfigurationBuilder, IOpcUaEntityServerInfo> serverSetup,
        Action<SimulationServicesConfig>? simulationSetup = null)
    {
        var simSetup = simulationSetup ?? (configure =>
        {
            configure.WithTimeScaling(double.Epsilon);
            configure.AdaptToRuntime<ServerSimulationAdapter>();
        });

        collection.AddOpcUaServerConfiguration(serverSetup)
            .WithEntityServices()
            .WithOpcUaClientServices()
            .WithOpcUaServer()
            .WithOpcUaSimulationServices(simSetup)
            .Collection.AddLogging(e => e.AddSimpleConsole());
        return collection;
    }
}