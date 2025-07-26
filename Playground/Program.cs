using System.Configuration;
using System.Text.Json;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Playground.Configuration;
using Playground.Reactants;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = true,
        MaxDepth = 10,
    };
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
});
builder.Services.AddHostedService<PositionChangeReactor>();
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var opcUaConfig = builder.Configuration.GetSection("OpcUa").Get<OpcUaOptions>();
if (opcUaConfig is null || opcUaConfig.Port == 0)
    throw new ConfigurationErrorsException("OpcUa configuration is missing");

builder.Services.AddOpcUaServerConfiguration(conf => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithOpcTcpHost("localhost", opcUaConfig.Port)
        .WithEndpoints([$"opc.tcp://localhost:{opcUaConfig.Port}"])
        .Build())
    .WithEntityServices()
    .WithOpcUaSimulationServices(config =>
    {
        config.WithTimeScaling(0.1);
        config.AdaptToRuntime<ServerSimulationAdapter>();
    })
    .WithOpcUaServerAsBackgroundService()
    .WithOpcUaClientServices();

var app = builder.Build();


app.UseHttpsRedirection();
await app.RunAsync();