using System.Collections.Immutable;
using FluentAssertions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.EntityRulesAnalysis.Diagnostics;
using Hoeyer.OpcUa.EntityRulesAnalysis.Fields.Properties;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit.Abstractions;

namespace Hoeyer.OpcUa.EntityAnalysis.Test.Fields.Properties;


[TestSubject(typeof(PropertiesMustBePublicAnalyser))]
public class PropertiesMustBePublicAnalyserTest(ITestOutputHelper helper)
    
{
    private readonly AnalyzerTestDriver<PropertiesMustBePublicAnalyser> _driver = new(new PropertiesMustBePublicAnalyser(), helper.WriteLine);


    [Theory]
    [ClassData(typeof(TestEntities.ValidData))]
    public async Task WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var res = await _driver.RunAnalyzerOn(sourceCode);
        res.Diagnostics.Should().BeEmpty();
    }
    
    [Theory]
    [ClassData(typeof(TestEntities.InvalidData))]
    public async Task WhenSourceCode_Violates_SupportedFieldPublicity_ShouldHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var res = await _driver.RunAnalyzerOn(sourceCode);
        res.Diagnostics.Should().Contain(e => e.Descriptor.Equals(OpcUaDiagnostics.MustHavePublicSetterDescriptor));
    }
}
