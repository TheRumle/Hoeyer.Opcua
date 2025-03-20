using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class PropertyAccessViolationsAttribute : DataSourceGeneratorAttribute<EntitySourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntitySourceCode>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return EntitySourceCodeTestSet.PropertyAccessViolations.Select(source => (Func<EntitySourceCode>)(() => source));
    }
}