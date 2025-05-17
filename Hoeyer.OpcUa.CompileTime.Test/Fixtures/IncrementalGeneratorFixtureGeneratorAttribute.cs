using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Client.SourceGeneration;
using Hoeyer.OpcUa.Core.SourceGeneration.Generation;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
internal sealed class
    IncrementalGeneratorFixtureGeneratorAttribute : DataSourceGeneratorAttribute<IIncrementalGenerator>
{
    /// <inheritdoc />
    public override IEnumerable<Func<IIncrementalGenerator>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        IEnumerable<Func<IIncrementalGenerator>> generatedCoreServices =
            new TypesWithEmptyCtorScanningGeneratorAttribute<IIncrementalGenerator,
                EntityTranslatorGenerator>().GenerateDataSources(dataGeneratorMetadata);

        IEnumerable<Func<IIncrementalGenerator>> generatedClientServices =
            new TypesWithEmptyCtorScanningGeneratorAttribute<IIncrementalGenerator,
                RemoteMethodCallerGenerator>().GenerateDataSources(dataGeneratorMetadata);

        return generatedClientServices.Concat(generatedCoreServices);
    }
}