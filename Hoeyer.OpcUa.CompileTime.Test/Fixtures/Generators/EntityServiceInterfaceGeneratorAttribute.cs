using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

public sealed class EntityServiceInterfaceGeneratorAttribute : DataSourceGeneratorAttribute<ServiceInterfaceSourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<ServiceInterfaceSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
        => EntitySourceCodeDefinitions.ValidEntities.SelectMany(TestBehaviours.GetServiceInterfaceSourceCodeFor)
            .SelectFunc();
}