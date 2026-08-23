using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.TUnitConfiguration.Logging;
using Hoeyer.OpcUa.Server.Configuration;
using Hoeyer.OpcUa.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public static class ServiceCollectionExtensions
{
    private static readonly string PkiRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OPC Foundation",
        "pki");

    public static OnGoingOpcEntityServiceRegistrationWithModels AddClientTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] clientModelMarker) =>
        services.AddClientTestServices(args, clientModelMarker, clientModelMarker);

    public static OnGoingOpcEntityServiceRegistrationWithModels AddClientTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker)
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
            .AddScoped<IServiceProvider>(p => p)
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
            .WithEntityModelsFrom(entityAssemblyMarkers)
            .WithOpcUaClientModelsFrom(clientModelMarker,
                c => { c.WithEntitySessionFactory<EntitySessionFactory>(); });
    }


    public static void AddClientAndServerTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker,
        Type[] serverModelMarker)
    {
        var clientServices = services.AddClientTestServices(args, entityAssemblyMarkers, clientModelMarker);
        clientServices.WithOpcUaServer(serverModelMarker);
    }

    public static OnGoingOpcEntityServerServiceRegistration AddClientAndServerTestServices(
        this IServiceCollection services,
        OpcEnvironment args,
        Type[] entityAssemblyMarkers) =>
        services.AddClientTestServices(args, entityAssemblyMarkers, entityAssemblyMarkers)
            .WithOpcUaServer(entityAssemblyMarkers);
}