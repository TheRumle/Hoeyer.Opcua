using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Playground.Modelling.Methods;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

internal sealed class ClientServiceCollection
{
    public ServiceCollection Collection;

    public ClientServiceCollection(string hostName, int port)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddSingleton(services)
            .AddScoped<IServiceProvider>(p => p)
            .AddOpcUa(conf => conf
                .WithServerId(OpcUaSimulationServerContainer.OPCUA_SERVERID)
                .WithServerName(OpcUaSimulationServerContainer.OPCUA_SERVERNAME)
                .WithWebOrigins(WebProtocol.OpcTcp, hostName, port)
                .Build())
            .WithEntityModelsFrom(typeof(Gantry))
            .WithOpcUaClientModelsFrom(typeof(IGantryMethods));

        Collection = services;
    }
}