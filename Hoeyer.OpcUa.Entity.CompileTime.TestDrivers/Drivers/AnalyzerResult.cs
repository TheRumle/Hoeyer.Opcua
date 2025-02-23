using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed record AnalyzerResult(IEnumerable<Diagnostic> Diagnostics, AnalyzerTelemetryInfo Statistics);