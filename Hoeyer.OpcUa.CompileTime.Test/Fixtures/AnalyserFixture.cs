using Hoeyer.OpcUa.Core.CompileTime;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

internal sealed class AnalyserFixture : DataSourceGeneratorAttribute<DiagnosticAnalyzer>
{
    private readonly TypesWithEmptyCtorScanner<DiagnosticAnalyzer, ConcurrentAnalyzer> _scanner = new();

    /// <inheritdoc />
    protected override IEnumerable<Func<DiagnosticAnalyzer>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata) => _scanner.GenerateDataSources();
}