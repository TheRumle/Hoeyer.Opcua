﻿using Hoeyer.Common.Reflection;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.SanityCheck;

public class CompileTimeTypeAnnotations
{
    [Test]
    [IncrementalGeneratorFixtureGenerator]
    public async Task IsAnnotatedWithGenerator(IIncrementalGenerator generator)
    {
        await Assert.That(generator.GetType().IsAnnotatedWith<GeneratorAttribute>()).IsTrue().Because(
            "if the class is not annotated with the attribute it will not be used doing source generation.");
    }


    [Test]
    [AnalyserFixtureGenerator]
    public async Task IsAnnotatedWithAnalyzer(DiagnosticAnalyzer analyzer)
    {
        await Assert.That(analyzer.GetType().IsAnnotatedWith<DiagnosticAnalyzerAttribute>()).IsTrue()
            .Because("if the class is not annotated with the attribute it will not be used doing source generation.");
    }
}