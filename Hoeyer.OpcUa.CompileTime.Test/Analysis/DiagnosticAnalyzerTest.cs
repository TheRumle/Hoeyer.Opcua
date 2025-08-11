using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

public abstract class DiagnosticAnalyzerTest<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected readonly TAnalyzer Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);

    [Test]
    [ValidAgentSourceCodeGenerator]
    [DisplayName("Does not report error for $agentSourceCode")]
    public async Task GivenValidAgent_ShouldNotHaveDiagnostic(AgentSourceCode agentSourceCode)
    {
        var res = await Driver.RunAnalyzerOn(agentSourceCode);
        var diagnosticsReportedByAnalyzer =
            res.Diagnostics.Where(diagnostic => Analyzer.SupportedDiagnostics.Contains(diagnostic.Descriptor));
        await Assert.That(diagnosticsReportedByAnalyzer).IsEmpty()
            .Because("Correct entities should not have diagnostics.");
    }
}