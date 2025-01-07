using FluentAssertions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.EntityGeneration.Generators;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Entity.Generation.Test.Generators;

[TestSubject(typeof(EntityNodeCreatorGenerator))]
public class EntityNodeCreatorGeneratorTest{
    
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver =  new (new EntityNodeCreatorGenerator());
    
    
    [Theory]
    [ClassData(typeof(TestEntities.ValidData))]
    public void WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode(sourceCode);
        generationResult.IsSuccess.Should().BeTrue();
        generationResult.Value.Diagnostics.Where(e=> e.Severity == DiagnosticSeverity.Error).Should().BeEmpty();
    }
    
    [Theory]
    [ClassData(typeof(TestEntities.NegativeData))]
    public void WhenGiven_InCorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode(sourceCode);
        generationResult.IsSuccess.Should().BeTrue();
        generationResult.Value.Diagnostics.Should().BeEmpty();
    }
}