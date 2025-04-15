using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;


internal sealed class
    AnalyserFixtureGeneratorAttribute : TypesWithEmptyCtorScanningGeneratorAttribute<DiagnosticAnalyzer,
    ConcurrentAnalyzer>;