using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(FieldMustBeOfSupportedTypeAnalyser))]
[InheritsTests]
public sealed class FieldMustBeOfSupportedTypeAnalyserTest : AnalyzerTest<FieldMustBeOfSupportedTypeAnalyser>
{
    
    [Test]
    [UnsupportedTypesSourceCodeGenerator]
    [DisplayName("Reports error for $entitySourceCode")]
    public async Task GivenEntityWithUnsupportedFields_ShouldHaveDiagnostic(EntitySourceCode entitySourceCode)
    {
        var res = await Driver.RunAnalyzerOn(entitySourceCode);
        var diagnosticsReportedByAnalyzer =
            res.Diagnostics.Where(diagnostic => Analyzer.SupportedDiagnostics.Contains(diagnostic.Descriptor));
        await Assert.That(diagnosticsReportedByAnalyzer).IsNotEmpty()
            .Because("If the field is not supported, a diagnostic should be reported.");
    }
    
}