using System.Diagnostics.CodeAnalysis;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class EntityFixtureGeneratorAttribute : DataSourceGeneratorAttribute<EntityReaderFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntityReaderFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return CreateServiceCollection(dataGeneratorMetadata)
            .SelectFunc(e =>
                new EntityReaderFixture(e.EntityNode, Enumerable.ToHashSet<PropertyState>(e.PropertyStates.Values)));
    }

    private static IEnumerable<EntityBrowserFixture> CreateServiceCollection(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return new EntityBrowserFixtureGeneratorAttribute()
            .GenerateDataSources(dataGeneratorMetadata)
            .Select(e => e.Invoke());
    }
}