using System.Configuration;
using System.Text.Json;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.EntityModelling;
using Hoeyer.OpcUa.EntityModelling.Reactants;
using Hoeyer.OpcUa.Simulation.Api.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
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
            simulationSetup.AdaptToRuntime<ServerLayerAdapter>();
        }
    );


builder.Services.AddSingleton<EntitiesContainer>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton(typeof(IStateChangeObserver<>), typeof(StateChangeObserver<>));
builder.Services.AddScoped(typeof(StateService<>));
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Map Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseHttpsRedirection();
await app.RunAsync();