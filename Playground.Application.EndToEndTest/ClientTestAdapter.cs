using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.Adapter;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest;

public sealed class ClientTestAdapter : ITestFrameworkAdapter
{
    public IOpcUaSimulationServer ConstructSimulationServer() =>
        new OpcUaServerTestContainer(WebProtocol.OpcTcp);

    public Type[] ClientAssemblyMarkers => [typeof(Gantry)];
    public Type[] EntityAssemblyMarkers => [typeof(IGantryMethods)];
}