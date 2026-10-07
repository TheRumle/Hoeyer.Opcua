using Hoeyer.OpcUa.Core.Compiletime;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Compiletime.Test.Fixtures;

internal sealed class AnalyserFixtureAttribute : DataSourceGeneratorAttribute<DiagnosticAnalyzer>
{
    private readonly TypesWithEmptyCtorScanner<DiagnosticAnalyzer, ConcurrentAnalyzer> _scanner = new();

    /// <inheritdoc />
    protected override IEnumerable<Func<DiagnosticAnalyzer>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata) => _scanner.GenerateDataSources();
}