using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed record AnalyzerResult(ImmutableArray<Diagnostic> Diagnostics, AnalyzerTelemetryInfo Statistics);