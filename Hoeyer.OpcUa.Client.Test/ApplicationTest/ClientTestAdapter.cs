using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;
using Hoeyer.OpcUa.Test.Simulation;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace OpcUa.Client.TestFramework.ApplicationTest;

public sealed class ClientTestAdapter : ITestFrameworkAdapter
{
    public IOpcUaSimulationServer ConstructSimulationServer() =>
        new OpcUaServerTestContainer(WebProtocol.OpcTcp);

    public Type[] ClientAssemblyMarkers => [typeof(Gantry)];
    public Type[] EntityAssemblyMarkers => [typeof(IGantryMethods)];
}