using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

public sealed class AgentServiceInterfaceGeneratorAttribute : DataSourceGeneratorAttribute<ServiceInterfaceSourceCode>
{
    protected override IEnumerable<Func<ServiceInterfaceSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
        => AgentSourceCodeDefinitions.ValidEntities.SelectMany(TestBehaviours.GetServiceInterfaceSourceCodeFor)
            .SelectFunc();
}