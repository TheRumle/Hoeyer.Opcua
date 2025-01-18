using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public record GeneratorResult(
    ImmutableArray<Diagnostic> Diagnostics,
    ImmutableArray<SyntaxTree> GeneratedTrees,
    TimeSpan TimingInformation)
{
    public IEnumerable<Diagnostic> Errors => Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);
}