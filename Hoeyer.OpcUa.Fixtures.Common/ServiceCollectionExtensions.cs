using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Fixtures.Common;

public static class ServiceCollectionExtensions
{
    private static readonly string PkiRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OPC Foundation",
        "pki");

    public static OnGoingOpcEntityServiceRegistrationWithModels AddCoreTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers)
    {
        return services
            .AddSingleton<TUnitLoggerProvider>()
            .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
            .AddSingleton(services)
            .AddSingleton<ILoggerFactory>(p =>
                LoggerFactory.Create(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddProvider(p.GetRequiredService<TUnitLoggerProvider>());
                }))
            .AddSingleton<ITelemetryContext>(p =>
            {
                var loggerFactory = p.GetRequiredService<TUnitLoggerProvider>();
                return DefaultTelemetry.Create(configure =>
                {
                    configure.AddProvider(loggerFactory).SetMinimumLevel(LogLevel.Debug);
                });
            })
            .AddOpcUa(conf => conf
                .WithServerId(args.OpcUaServerId)
                .WithServerName(args.OpcUaServerName)
                .WithWebOrigins(
                    args.Protocol,
                    args.HostName,
                    args.Port
                )
                .WithApplicationUri($"/{args.OpcUaServerName}")
                .WithSecurityConfiguration(
                    new CertificateConfiguration
                    {
                        PkiRoot = PkiRoot,
                        CertificateSubjectName = "CN=ABC",
                        OwnStorePath = Path.Combine(PkiRoot, "own"),
                        TrustedStorePath = Path.Combine(PkiRoot, "trusted"),
                        IssuerStorePath = Path.Combine(PkiRoot, "issuer"),
                        RejectedStorePath = Path.Combine(PkiRoot, "rejected")
                    })
                .Build())
            .WithEntityModelsFrom(entityAssemblyMarkers);
    }
}