using Hoeyer.Machines.OpcUa.Client.Application;
using Hoeyer.Machines.OpcUa.Client.Application.MachineProxy;
using Hoeyer.Machines.OpcUa.Client.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using MyOpcUaWebApplication;
using MyOpcUaWebApplication.Background;
using MyOpcUaWebApplication.Configuration.BackgroundService;
using MyOpcUaWebApplication.Configuration.OpcUa.Options;
using MyOpcUaWebApplication.Options;
using Opc.Ua.Client;
using Org.BouncyCastle.Tls;

var builder = WebApplication.CreateBuilder(args);

const string GantryOptionsErrorMessage =
    $"The appsettings json file does not configure {nameof(GantryOptions)} correctly. Compare the {nameof(GantryOptions)} class and the {GantryOptions.APPCONFIG_SECTION} section of the .json file.";


builder.Services
    .AddOptions<GantryOptions>()
    .Bind(builder.Configuration.GetSection(GantryOptions.APPCONFIG_SECTION))
    .Validate(e=>!string.IsNullOrEmpty(e.Name) && e.Speed > 0 && !string.IsNullOrEmpty(e.Id), GantryOptionsErrorMessage)
    .ValidateOnStart();

builder.Services
    .AddOptions<OpcUaRootConfigOptions>()
    .Bind(builder.Configuration.GetSection(OpcUaRootConfigOptions.OPCUA_ROOT_CONFIG_SECTION))
    .Validate(e => e.NamespaceIndex > 0, "Namespace index must be greater than 0")
    .ValidateOnStart();

builder.Services
    .AddOptions<GantryScannerOptions>()
    .Bind(builder.Configuration.GetSection(GantryScannerOptions.APPCONFIG_SECTION))
    .Validate(e=>e.IntervalMs > 0, $"{nameof(GantryScannerOptions.IntervalMs)} must be greater than 0")
    .ValidateOnStart();

builder.Services
    .AddOptions<OpcUaServerOptions>()
    .Bind(builder.Configuration.GetSection("OPCUA:server"))
    .Validate(e => !string.IsNullOrEmpty(e.ServerName), "Server name must be defined!")
    .Validate(e => !string.IsNullOrEmpty(e.url)
                   && Uri.TryCreate(e.url, UriKind.RelativeOrAbsolute, out var _),
        "Opc server options must have valid URI.")
    .ValidateOnStart();


builder.Services.AddOpcUaEntities();
builder.Services.AddHostedService<GantryScanner>();



var app = builder.Build();


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

app.Run();