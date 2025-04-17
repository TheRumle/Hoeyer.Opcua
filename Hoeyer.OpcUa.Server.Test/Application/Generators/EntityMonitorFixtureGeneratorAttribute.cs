using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Server.Test.Application.Fixtures;
using Hoeyer.OpcUa.Server.Test.Entities;

namespace Hoeyer.OpcUa.Server.Test.Application.Generators;

public sealed class EntityMonitorFixtureGeneratorAttribute : DataSourceGeneratorAttribute<EntityMonitorFixture>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntityMonitorFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return GeneratedTypes
            .EntityNodeCreators
            .SelectFunc(creator => new EntityMonitorFixture(creator));
    }
}