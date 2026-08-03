using Hoeyer.OpcUa.Client.Application.Connection;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.IntegrationTest.EnvironmentAdapter;
using Hoeyer.OpcUa.IntegrationTest.TUnitConfiguration.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.IntegrationTest.Fixtures;

public class ClientIntegrationServices : IDisposable
{
    public readonly OnGoingOpcEntityServiceRegistrationWithModels OpcServices;

    public ClientIntegrationServices(
        IServiceCollection services,
        ClientServicesAdapterArgs args,
        Type[] entityAssemblyMarkers,
        Type[] clientModelMarker
    ) : this(services, args, entityAssemblyMarkers.ToHashSet(), clientModelMarker.ToHashSet())
    {
    }

    public ClientIntegrationServices(
        IServiceCollection services,
        ClientServicesAdapterArgs args,
        ISet<Type> entityAssemblyMarkers,
        ISet<Type> clientModelMarker
    )
    {
        var pkiRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OPC Foundation",
            "pki");

        OpcServices = services.AddSingleton<ILoggerFactory>(_ =>
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
                        PkiRoot = pkiRoot,
                        CertificateSubjectName = "CN=ABC",
                        OwnStorePath = Path.Combine(pkiRoot, "own"),
                        TrustedStorePath = Path.Combine(pkiRoot, "trusted"),
                        IssuerStorePath = Path.Combine(pkiRoot, "issuer"),
                        RejectedStorePath = Path.Combine(pkiRoot, "rejected"),
                    })
                .Build())
            .WithEntityModelsFrom(entityAssemblyMarkers)
            .WithOpcUaClientModelsFrom(clientModelMarker,
                configure: c => { c.WithEntitySessionFactory<EntitySessionFactory>(); });

        Collection = services;
        ServiceProvider = Collection.BuildServiceProvider();
        Scope = ServiceProvider.CreateScope();
    }

    public IServiceCollection Collection { get; private set; }

    public IServiceScope Scope { get; private set; }

    public IServiceProvider ServiceProvider { get; private set; }

    public void Dispose()
    {
        Scope.Dispose();
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}