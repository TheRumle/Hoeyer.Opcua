using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

public record GenerationResult(
    ImmutableArray<Diagnostic> Diagnostics,
    ImmutableArray<SyntaxTree> GeneratedTrees,
    TimeSpan TimingInformation);