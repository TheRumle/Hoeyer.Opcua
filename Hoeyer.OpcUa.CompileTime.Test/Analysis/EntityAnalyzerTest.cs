using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(AgentAnalyzer))]
[InheritsTests]
public sealed class AgentAnalyzerTest : DiagnosticAnalyzerTest<AgentAnalyzer>
{
    [Test]
    [UnsupportedTypesSourceCodeGenerator]
    [DisplayName("Reports error for $agentSourceCode")]
    public async Task GivenAgentWithUnsupportedFields_ShouldHaveDiagnostic(AgentSourceCode agentSourceCode)
    {
        var res = await Driver.RunAnalyzerOn(agentSourceCode);
        var diagnosticsReportedByAnalyzer =
            res.Diagnostics.Where(diagnostic => Analyzer.SupportedDiagnostics.Contains(diagnostic.Descriptor));
        await Assert.That(diagnosticsReportedByAnalyzer).IsNotEmpty()
            .Because("If the field is not supported, a diagnostic should be reported.");
    }
}