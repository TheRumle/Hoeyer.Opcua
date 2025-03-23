using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures.Generator;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
internal sealed class
    AnalyserFixtureGeneratorAttribute : TypesWithEmptyCtorScanningGeneratorAttribute<DiagnosticAnalyzer,
    ConcurrentAnalyzer>;