using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit;

public abstract class ServiceCollectionInjectionAttribute(IServiceCollection? collection = null)
    : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    public IServiceCollection ServiceCollection { get; } = collection ?? new ServiceCollection();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceCollection.BuildServiceProvider().CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;
}