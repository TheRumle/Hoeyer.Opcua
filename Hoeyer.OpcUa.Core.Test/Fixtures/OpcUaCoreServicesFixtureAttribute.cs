using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Hoeyer.OpcUa.Core.Test.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Core.Test.Fixtures;

public class OpcUaCoreServicesFixtureAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public readonly OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration;

    public OpcUaCoreServicesFixtureAttribute()
    {
        var pkiRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OPC Foundation",
            "pki");

        var services = new ServiceCollection();
        OnGoingOpcEntityServiceRegistration = services.AddLogging(c => c.SetMinimumLevel(LogLevel.Warning))
            .AddOpcUa(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithWebOrigins(WebProtocol.OpcTcp, "localhost", 10)
                .WithApplicationUri("/MyServer")
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
            .WithEntityModelsFrom(typeof(AllPropertyTypesEntity));
    }

    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) =>
        ServiceCollection.BuildServiceProvider().GetService(type);
}