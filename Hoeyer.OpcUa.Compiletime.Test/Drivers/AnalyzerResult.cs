using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;

namespace Hoeyer.OpcUa.CompileTime.Test.Drivers;

public sealed record AnalyzerResult(IEnumerable<Diagnostic> Diagnostics, AnalyzerTelemetryInfo Statistics);