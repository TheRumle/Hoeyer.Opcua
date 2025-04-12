using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
internal sealed class
    AnalyserFixtureGeneratorAttribute : TypesWithEmptyCtorScanningGeneratorAttribute<DiagnosticAnalyzer,
    ConcurrentAnalyzer>;