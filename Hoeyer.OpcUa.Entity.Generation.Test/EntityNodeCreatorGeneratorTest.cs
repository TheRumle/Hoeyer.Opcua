using FluentAssertions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.EntityGeneration.OpcUa;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

public class EntityNodeCreatorGeneratorTest{
    
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver =  new (new EntityNodeCreatorGenerator());
    
    [Theory]
    [ClassData(typeof(TestEntities.ValidData))]
    public void WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics(string sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode(sourceCode);
        generationResult.IsSuccess.Should().BeTrue();
        generationResult.Value.Diagnostics.Should().BeEmpty();
    }
}