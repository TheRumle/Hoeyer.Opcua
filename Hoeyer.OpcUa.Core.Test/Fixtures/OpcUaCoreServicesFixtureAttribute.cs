using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Core.Test.Fixtures;

public class OpcUaCoreServicesFixtureAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public readonly OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration;

    public OpcUaCoreServicesFixtureAttribute()
    {
        ReservedPort reservedPort = new();
        var services = new ServiceCollection();
        OnGoingOpcEntityServiceRegistration = services.AddLogging(c => c.SetMinimumLevel(LogLevel.Warning))
            .AddOpcUaServerConfiguration(conf => conf
                .WithServerId("MyServer")
                .WithServerName("My Server")
                .WithHttpsHost("localhost", reservedPort.Port)
                .WithEndpoints([$"opc.tcp://localhost:{reservedPort.Port}"])
                .Build())
            .WithEntityServices();
    }

    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) =>
        ServiceCollection.BuildServiceProvider().GetService(type);
}