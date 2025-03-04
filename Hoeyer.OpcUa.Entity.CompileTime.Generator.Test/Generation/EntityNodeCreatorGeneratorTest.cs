using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Data;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using JetBrains.Annotations;
using TUnitSettings;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Generation;

[TestSubject(typeof(EntityNodeCreatorGenerator))]
[ParallelLimiter<ParallelLimit>]
public class EntityNodeCreatorGeneratorTest
{
    private readonly GeneratorTestDriver<EntityNodeCreatorGenerator> _testDriver = new(new EntityNodeCreatorGenerator(),
        Console.WriteLine);

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Can generate syntax tree for '$entitySourceCode'")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateSyntaxTrees(EntitySourceCode entitySourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(entitySourceCode);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("Source code should be generated.");
    }
    
    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName($"Generates {nameof(IEntityNodeCreator)} for $sourceCode")]
    public async Task WhenGiven_CorrectSourceCode_GeneratesIEntityNodeCreator(EntitySourceCode sourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(sourceCode);
        await Assert.That(generationResult.SourceCode).Contains($"IEntityNodeCreator<{sourceCode.Type.Name}>");
    }


    [Test]
    [EntitySourceCodeGenerator]
    [DisplayName("Will not produce any diagnostic")]
    public async Task Generator_ShouldNeverProduceDiagnostics(EntitySourceCode entitySourceCode)
    {
        var generationResult = _testDriver.RunGeneratorOn(entitySourceCode);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}