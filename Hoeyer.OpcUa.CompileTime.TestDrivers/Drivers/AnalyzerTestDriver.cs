using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed class AnalyzerTestDriver<T>(T analyzer, Action<string>? logger = null)
    where T : DiagnosticAnalyzer
{
    private readonly CompilationFactory _compilationFactory = new(nameof(AnalyzerTestDriver<T>), logger);

    public Task<AnalyzerResult> RunAnalyzerOn(EntitySourceCode entitySourceCode,
        CancellationToken cancellationToken = default) => CreateAnalyzerResultTask(
            trees: [CSharpSyntaxTree.ParseText(entitySourceCode.SourceCodeString)],
            cancellationToken: cancellationToken
        );

    public Task<AnalyzerResult> RunAnalyzerOn(
        EntityAndServiceSourceCode sourceCode,
        CancellationToken cancellationToken = default)
    {

        var left = CSharpSyntaxTree.ParseText(sourceCode.EntitySourceCode);
        var right = CSharpSyntaxTree.ParseText(sourceCode.ServiceSourceCode);
        
        return CreateAnalyzerResultTask([left, right], cancellationToken);
    }

    public sealed class InvalidTestSourceException(string s) : Exception(s); 

    private async Task<AnalyzerResult> CreateAnalyzerResultTask(SyntaxTree[] trees, CancellationToken cancellationToken)
    {
        foreach (SyntaxTree syntaxTree in trees)
        {
            logger?.Invoke(syntaxTree.ToString());
        }
        
        var compilation = _compilationFactory.CreateCompilation(trees);
        var errs = compilation.GetDiagnostics().Where(e => e.Severity == DiagnosticSeverity.Error).ToList();
        if (errs.Any())
        {
            throw new InvalidTestSourceException("There were compilation errors for the given syntax tree(s): \n\n " + errs.ToNewlineSeparatedString());
        }
        
        
        var analyzingCompilation = compilation.WithAnalyzers([analyzer]);
        var diagnostics = analyzingCompilation.GetAnalyzerDiagnosticsAsync(cancellationToken);
        var statistics = analyzingCompilation.GetAnalyzerTelemetryInfoAsync(analyzer, cancellationToken);
        await Task.WhenAll(diagnostics, statistics);

        return new AnalyzerResult(diagnostics.Result, statistics.Result);
    }
}