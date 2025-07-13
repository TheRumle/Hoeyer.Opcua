using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

public sealed class ValidEntitySourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<EntitySourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntitySourceCode>>
        GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata) =>
        EntitySourceCodeDefinitions.ValidEntities.SelectFunc();
}