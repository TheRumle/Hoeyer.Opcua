using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.Adapter;
using Microsoft.Extensions.DependencyInjection;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest;

public sealed class EndToEndContainerizedSimulationSetup(string? containerName = null) : ITestFrameworkAdapter
{
    public IOpcUaSimulationTarget SimulationTarget { get; set; } =
        new PlaygroundTestContainer(WebProtocol.OpcTcp, containerName);

    public IServiceCollection ApplicationServices => new ServiceCollection();

    public Type[] ClientAssemblyMarkers => [typeof(Gantry)];
    public Type[] EntityAssemblyMarkers => [typeof(IGantryMethods)];
}