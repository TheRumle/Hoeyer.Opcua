using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Playground.Modelling.Models;

namespace Hoeyer.OpcUa.Core.Test.Fixtures;

public class OpcUaCoreServicesFixtureAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public readonly OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration;

    public OpcUaCoreServicesFixtureAttribute()
    {
        ReservedPort reservedPort = new();
        var services = new ServiceCollection();
        OnGoingOpcEntityServiceRegistration = services.AddLogging(c => c.SetMinimumLevel(LogLevel.Warning))
            .AddOpcUa(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithWebOrigins(WebProtocol.OpcTcp, "localhost", reservedPort.Port)
                .Build())
            .WithEntityModelsFrom(typeof(Gantry));
    }

    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) =>
        ServiceCollection.BuildServiceProvider().GetService(type);
}