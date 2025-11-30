using System.Text.Json;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Playground.Modelling.Models;
using Playground.Server;
using Playground.SimulationServer.Containerized;

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

builder.AddOpcUaFromEnvironmentVariables()
    .WithEntityModelsFrom(assemblyMarkers: typeof(Gantry))
    .WithOpcUaServerAsBackgroundService(typeof(GantryLoader))
    .WithOpcUaSimulationServices(configure =>
    {
        configure.WithTimeScaling(TimeScaler.Identity);
        configure.AdaptToRuntime<OpcUaServerAdapter>();
    });

builder.Services
    .AddHealthChecks()
    .AddCheck<ServerStartedHealthCheckAdapter>("server_started");

var app = builder.Build();

app.MapHealthChecks("/health/server");
await app.RunAsync();