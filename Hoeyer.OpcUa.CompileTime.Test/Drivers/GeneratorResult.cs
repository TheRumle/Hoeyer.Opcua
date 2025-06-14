﻿using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Drivers;

public record GeneratorResult(
    IEnumerable<Diagnostic> Diagnostics,
    IEnumerable<SyntaxTree> GeneratedTrees,
    TimeSpan TimingInformation)
{
    public IEnumerable<Diagnostic> Errors => Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error);

    public string SourceCode => string.Join(Environment.NewLine, GeneratedTrees);
}