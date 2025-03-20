using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures.Generator;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Analysis;

public class MustBePublicAnalyserTest
{
    private readonly AnalyzerTestDriver<DiagnosticAnalyzer> _driver = new(new MustBePublicAnalyser());

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Does not report error for $entitySourceCode")]
    [NotInParallel]
    public async Task GivenValidEntity_ShouldNotHaveDiagnostic(EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        await Assert.That(res.Diagnostics).IsEmpty().Because("Correct entities should not have diagnostics.");
    }

    [Test]
    [PropertyAccessViolations]
    [DisplayName("Reports error for $entitySourceCode")]
    [NotInParallel]
    public async Task GivenInvalidEntity_ShouldHaveDiagnostic(
        EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        await Assert.That(res.Diagnostics).IsNotEmpty();
    }
}