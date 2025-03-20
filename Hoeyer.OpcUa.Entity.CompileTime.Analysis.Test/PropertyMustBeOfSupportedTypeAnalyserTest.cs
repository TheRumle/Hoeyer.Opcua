﻿using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Data;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.Analysis.Test;

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
        var s = string.Join("\n", res.Diagnostics.Select(e => e.ToString()));
        await Assert.That(s)
            .IsEmpty().Because("Correct entities should not have diagnostics.");
    }
}