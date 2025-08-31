using System.Configuration;
using System.Text.Json;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.TestEntities;
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

builder.Services
    .AddRunningTestEntityServices(
        serverSetup => serverSetup
            .WithServerId("MyServer")
            .WithServerName("My Server")
            .WithOpcTcpHost("localhost", opcUaConfig.Port)
            .WithEndpoints([$"opc.tcp://localhost:{opcUaConfig.Port}"])
            .Build(),
        simulationSetup =>
        {
            simulationSetup.WithTimeScaling(TimeScaler.Identity);
            simulationSetup.AdaptToRuntime<ServerSimulationAdapter>();
        }
    );

var app = builder.Build();


app.UseHttpsRedirection();
await app.RunAsync();