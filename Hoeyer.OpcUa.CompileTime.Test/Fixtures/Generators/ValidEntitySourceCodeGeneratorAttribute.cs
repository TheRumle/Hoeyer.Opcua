using Hoeyer.Common.Extensions.Collection;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

public sealed class ValidAgentSourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<AgentSourceCode>
{
    protected override IEnumerable<Func<AgentSourceCode>>
        GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata) =>
        AgentSourceCodeDefinitions.ValidEntities.SelectFunc();
}