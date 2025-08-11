using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

public abstract class DiagnosticAnalyzerTest<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected readonly TAnalyzer Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Does not report error for $entitySourceCode")]
    public async Task GivenValidEntity_ShouldNotHaveDiagnostic(EntitySourceCode entitySourceCode)
    {
        var res = await Driver.RunAnalyzerOn(entitySourceCode);
        var diagnosticsReportedByAnalyzer =
            res.Diagnostics.Where(diagnostic => Analyzer.SupportedDiagnostics.Contains(diagnostic.Descriptor));
        await Assert.That(diagnosticsReportedByAnalyzer).IsEmpty()
            .Because("Correct entities should not have diagnostics.");
    }
}