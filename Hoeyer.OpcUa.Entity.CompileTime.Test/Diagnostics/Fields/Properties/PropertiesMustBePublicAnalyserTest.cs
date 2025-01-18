using Hoeyer.OpcUa.CompileTime.Diagnostics;
using Hoeyer.OpcUa.CompileTime.Diagnostics.Fields.Properties;
using Hoeyer.OpcUa.Entity.Analysis.Test.Data;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Diagnostics.Fields.Properties;

[TestSubject(typeof(PropertiesMustBePublicAnalyser))]
public class PropertiesMustBePublicAnalyserTest
{
    private static readonly Func<Diagnostic, bool> CompareDiagnostic = descriptor =>
        OpcUaDiagnostics.MustHavePublicSetterDescriptor.Equals(descriptor.Descriptor);

    private readonly AnalyzerTestDriver<PropertiesMustBePublicAnalyser> _driver =
        new(new PropertiesMustBePublicAnalyser());

    [Test]
    [ValidEntitySourceCodeGenerator]
    public async Task WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        await Assert.That(res.Diagnostics).IsEmpty().Because("Correct entities should not have diagnostics.");
    }

    [Test]
    [InvalidEntitySourceCodeGenerator]
    public async Task WhenSourceCode_Violates_SupportedFieldPublicity_ShouldHaveDiagnostics(
        EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        await Assert.That(res.Diagnostics).IsNotEmpty().And.Contains(CompareDiagnostic);
    }
}