using System.Diagnostics.CodeAnalysis;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Hoeyer.OpcUa.Server.Test.Entities;

namespace Hoeyer.OpcUa.Server.Test.Application.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class EntityFixtureGeneratorAttribute : DataSourceGeneratorAttribute<EntityFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntityFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return GeneratedTypes
            .EntityNodeCreators
            .SelectFunc(creator => new EntityFixture(creator.Create(231)));
    }
}