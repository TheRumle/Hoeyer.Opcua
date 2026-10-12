using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Fixtures.Common;
using Hoeyer.OpcUa.Fixtures.Core.Fixtures.TestEntities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Core.Fixtures;

public class OpcUaCoreServicesFixtureAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public readonly OnGoingOpcEntityServiceRegistration OnGoingOpcEntityServiceRegistration;

    public OpcUaCoreServicesFixtureAttribute()
    {
        var services = new ServiceCollection();
        var env = OpcEnvironment.Default(10, "localhost");
        OnGoingOpcEntityServiceRegistration = services
            .AddCoreTestServices(env, [typeof(AllPropertyTypesEntity)]);
    }

    public IServiceCollection ServiceCollection => OnGoingOpcEntityServiceRegistration.Collection;

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateScope();

    public override object? Create(IServiceScope scope, Type type) =>
        ServiceCollection.BuildServiceProvider().GetService(type);
}