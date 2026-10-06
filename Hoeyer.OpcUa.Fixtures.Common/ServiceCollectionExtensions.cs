using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Fixtures.Common;

public static class ServiceCollectionExtensions
{
    public static OnGoingOpcEntityServiceRegistrationWithModels AddCoreServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers)
    {
        return services.AddSingleton<ILoggerFactory>(_ =>
                LoggerFactory.Create(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddProvider(new TUnitLoggerProvider());
                }))
            .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
            .AddSingleton(services)
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
    
    private static readonly string PkiRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OPC Foundation",
        "pki");

}