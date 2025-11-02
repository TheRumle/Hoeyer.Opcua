using System.Text.Json;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Playground.Modelling.Models;
using Playground.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = true,
        MaxDepth = 10
    };
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
});

builder.Services.AddLogging(e => e.AddJsonConsole(c => c.JsonWriterOptions = new JsonWriterOptions
{
    MaxDepth = 10
}));
builder.AddOpcUaFromEnvironmentVariables("OpcUaServer")
    .WithEntityModelsFrom(assemblyMarkers: typeof(Gantry))
    .WithOpcUaServerAsBackgroundService(typeof(GantryLoader))
    .WithOpcUaSimulationServices(configure =>
    {
        configure.WithTimeScaling(TimeScaler.Identity);
        configure.AdaptToRuntime<OpcUaServerAdapter>();
    });


var app = builder.Build();
await app.RunAsync();