using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
internal sealed class AnalyserFixtureGeneratorAttribute : DataSourceGeneratorAttribute<DiagnosticAnalyzer>
{
    /// <inheritdoc />
    public override IEnumerable<Func<DiagnosticAnalyzer>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return Analyzers.Select(analyzer => (Func<DiagnosticAnalyzer>)(() => analyzer));
    }

    private static readonly IEnumerable<DiagnosticAnalyzer> Analyzers = GetAnalyzers();
    private static IEnumerable<DiagnosticAnalyzer> GetAnalyzers()
    {
        return typeof(ConcurrentAnalyzer).Assembly
            .GetTypes()
            .Where(t => typeof(DiagnosticAnalyzer).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
            .ToHashSet()
            .Select(analyzerType => (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType)!)
            .ToList();
    }
}