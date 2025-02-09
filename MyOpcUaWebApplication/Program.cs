using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using MyOpcUaWebApplication;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using MyOpcUaWebApplication.Configuration.OpcUa.Options;
using MyOpcUaWebApplication.Configuration.Validation;

var builder = WebApplication.CreateBuilder(args);

const string GantryOptionsErrorMessage =
    $"The appsettings json file does not configure {nameof(GantryOptions)} correctly. Compare the {nameof(GantryOptions)} class and the {GantryOptions.APPCONFIG_SECTION} section of the .json file.";


builder.Services
    .AddOptions<GantryOptions>()
    .Bind(builder.Configuration.GetSection(GantryOptions.APPCONFIG_SECTION))
    .Validate(e => !string.IsNullOrEmpty(e.Name) && e.Speed > 0 && !string.IsNullOrEmpty(e.Id),
        GantryOptionsErrorMessage)
    .ValidateOnStart();

builder.Services
    .AddOptions<OpcUaRootConfigOptions>()
    .Bind(builder.Configuration.GetSection(OpcUaRootConfigOptions.OPCUA_ROOT_CONFIG_SECTION))
    //.Validate(e => e.NamespaceIndex > 0, "Namespace index must be greater than 0")
    .ValidateOnStart();

builder.Services
    .AddOptions<GantryScannerOptions>()
    .Bind(builder.Configuration.GetSection(GantryScannerOptions.APPCONFIG_SECTION))
    .Validate(e => e.IntervalMs > 0, $"{nameof(GantryScannerOptions.IntervalMs)} must be greater than 0")
    .ValidateOnStart();

builder.Services
    .AddOptions<OpcUaApplicationOptions>()
    .Bind(builder.Configuration.GetSection("OpcUa:Application"))
    .Validate(e => !string.IsNullOrWhiteSpace(e.ApplicationName), "ApplicationName name must be defined!")
    .Validate(e => !string.IsNullOrWhiteSpace(e.ApplicationUri) != default, "ApplicationUri must be defined!")
    .Validate(e => Uri.TryCreate(e.ApplicationUri, UriKind.Absolute, out _), "\nApplicationUri must be absolute URI.\n")
    .ValidateBeforeStart();

builder.Services.AddOpcUaEntityServer((conf) => conf
        .WithServerId("MyServer")
        .WithServerName("My Server")
        .WithHost("localhost")
        .WithEndpoints(["opc.tcp://localhost:4840"])
        .Build())
    .WithEntityNodeGeneration();

builder.Services.AddOpcUaClientServices();

var app = builder.Build();

var factory = app.Services.GetService<OpcUaEntityServerFactory>()!;
await factory.CreateServer().StartAsync();


app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

await app.RunAsync();