﻿using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Fixtures.Generator;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Entity.CompileTime.Test.Analysis;

[TestSubject(typeof(EntityContainerGenerator))]
public class PropertyMustBeOfSupportedTypeAnalyserTest
{
    private readonly AnalyzerTestDriver<PropertyMustBeOfSupportedTypeAnalyser> _driver
        = new(new PropertyMustBeOfSupportedTypeAnalyser(), Console.WriteLine);

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Does not report error for $entitySourceCode")]
    [NotInParallel]
    public async Task GivenValidEntity_ShouldNotHaveDiagnostic(EntitySourceCode entitySourceCode)
    {
        var res = await _driver.RunAnalyzerOn(entitySourceCode);
        var diagnosticDescriptions = string.Join("\n", res.Diagnostics.Select(e => e.ToString()));
        await Assert.That(diagnosticDescriptions)
            .IsEmpty()
            .Because("correct entities should not have diagnostics.");
    }
}