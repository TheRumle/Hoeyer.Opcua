using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Data;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class InvalidEntitySourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<EntitySourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntitySourceCode>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return EntitySourceCodeTestSet.Invalid.Select(source => (Func<EntitySourceCode>)(() => source));
    }
}