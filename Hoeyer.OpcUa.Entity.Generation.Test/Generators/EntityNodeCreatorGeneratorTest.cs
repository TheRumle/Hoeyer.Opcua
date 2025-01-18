﻿using FluentAssertions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.EntityGeneration.Generators;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Hoeyer.OpcUa.Entity.Generation.Test.Generators;

[TestSubject(typeof(EntityNodeCreatorGenerator))]
public class EntityNodeCreatorGeneratorTest {
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver;

    public EntityNodeCreatorGeneratorTest(ITestOutputHelper output)
    {
        _testDriver = new(new EntityNodeCreatorGenerator(), output.WriteLine);
    }
    
    [Theory]
    [ClassData(typeof(TestEntities.ValidData))]
    public void WhenGiven_CorrectSourceCodeInfo_ShouldCreate_XX(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(sourceCode);
        generationResult.GeneratedTrees.Should().NotBeEmpty("Source code should be generated.");
    }
    
    [Theory]
    [ClassData(typeof(TestEntities.AllData))]
    public void ShouldNotProduceDiagnostics(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(sourceCode);
        generationResult.Errors.Should().BeEmpty("The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}