using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;
using Playground.Server;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

public static class ServiceCollectionTestExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services,
        WebProtocol protocol = WebProtocol.OpcTcp, int port = 5)
    {
        return services
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddOpcUa(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithWebOrigins(protocol, "localhost", port)
                .Build())
            .WithEntityModelsFrom(typeof(Gantry))
            .WithOpcUaClientModelsFrom(typeof(IGantryMethods))
            .WithOpcUaServer(typeof(GantryLoader))
            .WithOpcUaSimulationServices(c => c.AdaptToRuntime<OpcUaServerAdapter>())
            .Collection.AddSingleton(services)
            .AddScoped<IServiceProvider>(p => p);
    }
}