using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Server;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using MyOpcUaWebApplication.Configuration.BackgroundService;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddOptions<GantryScannerOptions>()
    .Bind(builder.Configuration.GetSection(GantryScannerOptions.APPCONFIG_SECTION))
    .Validate(e => e.IntervalMs > 0, $"{nameof(GantryScannerOptions.IntervalMs)} must be greater than 0")
    .ValidateOnStart();

builder.Logging.AddConsole();



builder.Services.AddOpcUaServerConfiguration(conf => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithHttpsHost("localhost", 4840)
        .WithEndpoints(["opc.tcp://localhost:4840"])
        .Build())
    .AddEntityOpcUaServer()
    .WithAutomaticEntityNodeCreation()
    .AddOpcUaClientServices();

var app = builder.Build();

var factory = app.Services.GetService<OpcUaEntityServerFactory>()!;
var server = factory.CreateServer();

await server.StartAsync();

app.UseHttpsRedirection();
await app.RunAsync();

