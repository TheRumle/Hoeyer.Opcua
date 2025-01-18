using Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed class AnalyzerTestDriver<T>(T analyzer, Action<string>? logger = null)
    where T : DiagnosticAnalyzer
{
    private readonly CompilationFactory _compilationFactory = new(nameof(AnalyzerTestDriver<T>), logger);
    
    public async Task<AnalyzerResult> RunAnalyzerOn(SourceCodeInfo sourceCodeInfo, CancellationToken cancellationToken = default)
    {
        var compilation = _compilationFactory.CreateCompilation(sourceCodeInfo).WithAnalyzers([analyzer]);
        var diagnostics = compilation.GetAnalyzerDiagnosticsAsync(cancellationToken);
        var statistics = compilation.GetAnalyzerTelemetryInfoAsync(analyzer, cancellationToken);
        await Task.WhenAll(diagnostics, statistics);

        return new AnalyzerResult(await diagnostics, await statistics);
    }
}