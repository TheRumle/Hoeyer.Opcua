using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

public abstract class ConcurrentAnalyzer(ImmutableArray<DiagnosticDescriptor> descriptors) : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = descriptors;
    
    /// <inheritdoc />
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        InitializeAnalyzer(context);
    }

    protected abstract void InitializeAnalyzer(AnalysisContext context);
}