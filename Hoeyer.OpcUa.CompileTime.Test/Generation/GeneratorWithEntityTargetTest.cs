using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[InheritsTests]
public abstract class GeneratorWithEntityTargetTest(IIncrementalGenerator generator)
{
    protected readonly GeneratorTestDriver TestDriver = new(generator, Console.WriteLine);

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Can generate valid syntax tree for '$entitySourceCode'")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateValidSyntaxTrees(EntitySourceCode entitySourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(entitySourceCode.SourceCodeString);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");
    }

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Generates valid syntax tree for $sourceCode")]
    public async Task WhenGivenValidSourceCode_ProducesValidSyntaxTree(EntitySourceCode sourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(sourceCode.SourceCodeString);
        var syntaxTree = CSharpSyntaxTree.ParseText(generationResult.SourceCode);
        var diagnostics = syntaxTree.GetDiagnostics().ToList();
        await Assert.That(diagnostics).IsEmpty()
            .Because(
                $" the generated syntax trees should not have compilation errors: {string.Join('\n', diagnostics)}");
    }

    [Test]
    [ValidEntitySourceCodeGenerator]
    [DisplayName("Will not produce any diagnostic for valid class fixtures")]
    public async Task Generator_ShouldNeverProduceDiagnostics(EntitySourceCode entitySourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(entitySourceCode.SourceCodeString);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}