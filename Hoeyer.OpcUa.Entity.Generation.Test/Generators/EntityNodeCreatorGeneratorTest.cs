using FluentAssertions;
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
        _testDriver = new(new EntityNodeCreatorGenerator(), output);
    }
    
    
    
    [Theory]
    [ClassData(typeof(TestEntities.ValidData))]
    public void WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode(sourceCode);
        generationResult.Value.Errors.Should().BeEmpty();
    }
    
    [Theory]
    [ClassData(typeof(TestEntities.NegativeData))]
    public void WhenGiven_InCorrectEntitySourceCode_ShouldNotHaveDiagnostics(SourceCodeInfo sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode(sourceCode);
        generationResult.Value.Errors.Should().NotBeEmpty();
    }
}