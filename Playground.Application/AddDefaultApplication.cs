using System.Text.Json;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.Application;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Server.Services;
using Hoeyer.OpcUa.Simulation.Abstractions.Services;
using Hoeyer.OpcUa.Simulation.ServerAdapter;
using Hoeyer.OpcUa.Simulation.Services;
using Opc.Ua;
using Playground.Application.host.healthcheck;
using Playground.Clients;
using Playground.Modelling.Models;
using Playground.Server;

namespace Playground.Application;

public static class AddDefaultApplicationExtension
{
    public static void AddDefaultSimulationApplication(this IHostApplicationBuilder builder,
        Func<IApplicationTargetConfigurationBuilder, IApplicationConfigurationRequirements>? configurationBuilder =
            null,
        ApplicationConfigurationSetup? configureOpcUaDefaults = null,
        Action<IServiceProvider, ApplicationConfiguration>? serverConfiguration = null,
        bool useEnvironmentVariables = false
    )
    {
        builder.Logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.JsonWriterOptions = new JsonWriterOptions
            {
                Indented = true,
                MaxDepth = 10,
            };
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
            options.IncludeScopes = true;
        });

        var pkiRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OPC Foundation",
            "pki");

        var config = configurationBuilder ?? ((serverSetup) => serverSetup
            .WithServerId("MyServer")
            .WithServerName("My Server")
            .WithWebOrigins(WebProtocol.OpcTcp, "localhost", 4840)
            .WithApplicationUri("/myApplication")
            .WithSecurityConfiguration(
                new CertificateConfiguration
                {
                    PkiRoot = pkiRoot,
                    CertificateSubjectName = "CN=ABC",
                    OwnStorePath = Path.Combine(pkiRoot, "own"),
                    TrustedStorePath = Path.Combine(pkiRoot, "trusted"),
                    IssuerStorePath = Path.Combine(pkiRoot, "issuer"),
                    RejectedStorePath = Path.Combine(pkiRoot, "rejected"),
                })
            .Build());

        var registration = useEnvironmentVariables
            ? builder.AddOpcUaFromEnvironmentVariables(configureOpcUaDefaults: configureOpcUaDefaults)
            : builder.Services.AddOpcUa(config);

        registration.WithEntityModelsFrom(typeof(Gantry))
            .WithOpcUaClientModelsFrom(typeof(PositionChangeReactor))
            .WithOpcUaServerAsBackgroundService(typeof(AllPropertiesLoader), (provider, configuration) => { })
            .WithOpcUaSimulationServices(configure =>
            {
                configure.WithTimeScaling(TimeScaler.Identity);
                configure.AdaptToRuntime<OpcUaServerAdapter>();
            });

        builder.Services
            .AddHealthChecks()
            .AddCheck<ServerStartedHealthCheckAdapter>("server_started", tags: new[] { "server" },
                timeout: TimeSpan.FromSeconds(15));
    }
}