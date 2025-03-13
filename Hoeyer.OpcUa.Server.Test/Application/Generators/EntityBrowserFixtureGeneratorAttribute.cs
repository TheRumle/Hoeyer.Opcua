using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Hoeyer.OpcUa.Test;
using TUnitSettings.Extensions;

namespace Hoeyer.OpcUa.Server.Test.Application.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class EntityBrowserFixtureGeneratorAttribute : DataSourceGeneratorAttribute<EntityBrowserFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntityBrowserFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return GeneratedTypes
            .EntityNodeCreators
            .SelectFunc(creator => new EntityBrowserFixture(creator));
    }
}