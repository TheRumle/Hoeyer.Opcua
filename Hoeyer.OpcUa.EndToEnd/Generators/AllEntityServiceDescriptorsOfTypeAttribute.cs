using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.EndToEndTest.Generators;

public sealed class AllEntityServiceDescriptorsOfTypeAttribute(Type type) : DataSourceGeneratorAttribute<IReadOnlyCollection<ServiceDescriptor>> 
{
    /// <inheritdoc />
    public override IEnumerable<Func<IReadOnlyCollection<ServiceDescriptor>>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var services = new AllOpcUaServicesFixture().Services;
        yield return () => new FilteredCollection(services, type).Descriptors.ToList();
    }
}