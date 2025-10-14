using System.Configuration;
using System.Text.Json;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;
using Hoeyer.OpcUa.Core.Services;
using Hoeyer.OpcUa.EntityModelling.Reactants;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Playground.Application;
using Playground.Application.Configuration;

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
builder.Services.AddHostedService<RandomContainerAssignmentReactor>();
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var opcUaConfig = builder.Configuration.GetSection("OpcUa").Get<OpcUaOptions>();
if (opcUaConfig is null || opcUaConfig.Port == 0)
    throw new ConfigurationErrorsException("OpcUa configuration is missing");

builder.Services.AddLogging(e => e.AddSimpleConsole());
builder.Services.AddOpcUa(serverSetup => serverSetup
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithWebOrigins(WebProtocol.Https, "localhost", 8080)
        .WithApplicationUri("/myApplication")
        .Build())
    .WithEntityServices()
    .WithOpcUaClientServices()
    .WithOpcUaServerAsBackgroundService()
    .WithOpcUaSimulationServices(configure =>
    {
        configure.WithTimeScaling(TimeScaler.Identity);
        configure.AdaptToRuntime<OpcUaServerAdapter>();
    });


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton(typeof(IStateChangeObserver<>), typeof(StateChangeObserver<>));
builder.Services.AddScoped(typeof(EntityStateService<>));
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Map Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseHttpsRedirection();
await app.RunAsync();