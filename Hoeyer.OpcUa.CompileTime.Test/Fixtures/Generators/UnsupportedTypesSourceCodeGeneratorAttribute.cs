using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class UnsupportedTypesSourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<AgentSourceCode>
{
    protected override IEnumerable<Func<AgentSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return AgentSourceCodeDefinitions.UnsupportedTypes.Select(source => (Func<AgentSourceCode>)(() => source));
    }
}