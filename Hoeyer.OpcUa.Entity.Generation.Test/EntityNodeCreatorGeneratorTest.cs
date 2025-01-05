using FluentAssertions;
using Hoeyer.OpcUa.EntityGeneration.OpcUa;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

public class EntityNodeCreatorGeneratorTest{
    
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver =  new (new EntityNodeCreatorGenerator()); 


    [Fact]
    public void WhenGiven_CorrectEntitySourceCode_ShouldNotHaveDiagnostics()
    {
        var generationResult = _testDriver.RunGeneratorOnSourceCode("HELLtsratO");
        generationResult.IsSuccess.Should().BeTrue();
        generationResult.Value.Diagnostics.Should().BeEmpty();
    }
}