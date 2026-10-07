using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.Compiletime.Test.Fixtures.EntityDefinitions;

namespace Hoeyer.OpcUa.Compiletime.Test.Fixtures.Generators;

public sealed class ValidEntitySourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<EntitySourceCode>
{
    protected override IEnumerable<Func<EntitySourceCode>>
        GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata) =>
        EntitySourceCodeDefinitions.ValidEntities.SelectFunc();
}