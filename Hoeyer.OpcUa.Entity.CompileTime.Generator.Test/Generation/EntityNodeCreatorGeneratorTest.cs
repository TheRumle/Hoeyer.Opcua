using Hoeyer.OpcUa.Entity.Analysis.Test.Data;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Generation;

[TestSubject(typeof(EntityNodeCreatorGenerator))]
public class EntityNodeCreatorGeneratorTest
{
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver = new(new EntityNodeCreatorGenerator(),
        Console.WriteLine);

    [Test]
    [ValidEntitySourceCodeGenerator]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateSyntaxTrees(EntitySourceCode entitySourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(entitySourceCode);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("Source code should be generated.");
    }


    [Test]
    [EntitySourceCodeGenerator]
    public async Task Generator_ShouldNeverProduceDiagnostics(EntitySourceCode entitySourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(entitySourceCode);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}