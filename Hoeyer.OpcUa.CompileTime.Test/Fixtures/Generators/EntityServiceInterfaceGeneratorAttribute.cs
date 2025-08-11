using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

public sealed class EntityServiceInterfaceGeneratorAttribute : DataSourceGeneratorAttribute<ServiceInterfaceSourceCode>
{
    protected override IEnumerable<Func<ServiceInterfaceSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
        => EntitySourceCodeDefinitions.ValidEntities.SelectMany(TestBehaviours.GetServiceInterfaceSourceCodeFor)
            .SelectFunc();
}