using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed class AnalyzerTestDriver<T>(T analyzer, Action<string>? logger = null)
    where T : DiagnosticAnalyzer
{
    private readonly CompilationFactory _compilationFactory = new(nameof(AnalyzerTestDriver<T>), logger);

    public async Task<AnalyzerResult> RunAnalyzerOn(EntitySourceCode entitySourceCode,
        CancellationToken cancellationToken = default)
    {
        var compilation = _compilationFactory.CreateCompilation(entitySourceCode).WithAnalyzers([analyzer]);
        var diagnostics = compilation.GetAnalyzerDiagnosticsAsync(cancellationToken);
        var statistics = compilation.GetAnalyzerTelemetryInfoAsync(analyzer, cancellationToken);
        await Task.WhenAll(diagnostics, statistics);

        return new AnalyzerResult(diagnostics.Result, statistics.Result);
    }
    
    public async Task<AnalyzerResult> RunAnalyzerOn(
        EntityAndServiceSourceCode sourceCode,
        CancellationToken cancellationToken = default)
    {
        var compilation = _compilationFactory
            .CreateCompilation(sourceCode.CombinedSourceCode)
            .WithAnalyzers([analyzer]);
        
        var diagnostics = compilation.GetAnalyzerDiagnosticsAsync(cancellationToken);
        var statistics = compilation.GetAnalyzerTelemetryInfoAsync(analyzer, cancellationToken);
        await Task.WhenAll(diagnostics, statistics);

        return new AnalyzerResult(diagnostics.Result, statistics.Result);
    }
}