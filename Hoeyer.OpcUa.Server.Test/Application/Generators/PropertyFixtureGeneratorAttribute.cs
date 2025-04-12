using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;

namespace Hoeyer.OpcUa.Server.Test.Application.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class PropertyFixtureGeneratorAttribute : DataSourceGeneratorAttribute<PropertyReaderFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<PropertyReaderFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return CreateServiceCollection(dataGeneratorMetadata)
            .SelectMany(serviceCollection => serviceCollection.PropertyStates,
                (serviceCollection, property) => (Func<PropertyReaderFixture>)(() =>
                    new PropertyReaderFixture(property.Value, serviceCollection.EntityName)));
    }

    private static IEnumerable<EntityBrowserFixture> CreateServiceCollection(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return new EntityBrowserFixtureGeneratorAttribute()
            .GenerateDataSources(dataGeneratorMetadata)
            .Select(e => e.Invoke());
    }
}