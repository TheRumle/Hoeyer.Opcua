using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using Opc.Ua;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddOptions<GantryScannerOptions>()
    .Bind(builder.Configuration.GetSection(GantryScannerOptions.APPCONFIG_SECTION))
    .Validate(e => e.IntervalMs > 0, $"{nameof(GantryScannerOptions.IntervalMs)} must be greater than 0")
    .ValidateOnStart();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}








builder.Services.AddOpcUaServerConfiguration(conf => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithHttpHost("localhost")
        .WithEndpoints(["opc.tcp://localhost:4840"])
        .Build())
    .WithEntityServerArgs((ServerConfiguration configuration)  => configuration.DiagnosticsEnabled = true )
    .WithEntityNodeGeneration()
    .AddOpcUaClientServices();

var app = builder.Build();

var factory = app.Services.GetService<OpcUaEntityServerFactory>()!;
var server = factory.CreateServer();

var loggerFactory = app.Services.GetService<ILoggerFactory>()!;
var logger = loggerFactory.CreateLogger("Console");
logger.LogError("HEltsiroatnsareio");

await server.StartAsync();

app.UseHttpsRedirection();
await app.RunAsync();

