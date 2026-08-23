using Hoeyer.Common.Extensions.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Test.Fixtures;

public class CoreServiceInjection : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private readonly IServiceProvider _serviceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        _serviceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;

    private static IServiceProvider CreateSharedServiceProvider() => CreateServiceCollection().BuildServiceProvider();

    private static IServiceCollection CreateServiceCollection()
    {
        OpcUaCoreServicesFixtureAttribute fixtureAttribute = new();
        var services = fixtureAttribute.ServiceCollection.SelectMany(CopiedServiceDescriptor).ToList();
        var collectionResult = new ServiceCollection();
        collectionResult.AddRange(services);
        return collectionResult;
    }

    private static IEnumerable<ServiceDescriptor> CopiedServiceDescriptor(ServiceDescriptor e)
    {
        if (e.ImplementationType is not null)
        {
            yield return new ServiceDescriptor(e.ImplementationType, e.ImplementationType, e.Lifetime);
        }

        yield return e;
    }
}