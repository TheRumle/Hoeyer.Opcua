using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Data;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.Analysis.Test;

public class PropertiesMustNotBeNullableAnalyserTest
{
    private readonly AnalyzerTestDriver<DiagnosticAnalyzer> _driver = new(new PropertiesMustNotBeNullableAnalyser());

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Does not report error for $entitySourceCode")]
    [NotInParallel]
    public async Task GivenValidEntity_ShouldNotHaveDiagnostic(EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        await Assert.That(res.Diagnostics).IsEmpty().Because("Correct entities should not have diagnostics.");
    }
}