﻿using FluentAssertions;
using Hoeyer.OpcUa.CompileTime.Diagnostics;
using Hoeyer.OpcUa.CompileTime.Diagnostics.Fields.Properties;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Diagnostics.Fields.Properties;


[TestSubject(typeof(PropertiesMustBePublicAnalyser))]
public class PropertiesMustBePublicAnalyserTest(ITestOutputHelper helper)
    
{
    private readonly AnalyzerTestDriver<PropertiesMustBePublicAnalyser> _driver = new(new PropertiesMustBePublicAnalyser(), helper.WriteLine);


    [Theory]
    [ClassData(typeof(TheoryDataEntities.ValidData))]
    public async Task WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var res = await _driver.RunAnalyzerOn(sourceCode);
        res.Diagnostics.Should().BeEmpty();
    }
    
    [Theory]
    [ClassData(typeof(TheoryDataEntities.InvalidData))]
    public async Task WhenSourceCode_Violates_SupportedFieldPublicity_ShouldHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var res = await _driver.RunAnalyzerOn(sourceCode);
        res.Diagnostics.Should().Contain(e => e.Descriptor.Equals(OpcUaDiagnostics.MustHavePublicSetterDescriptor));
    }
}
