using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[InheritsTests]
public abstract class GeneratorWithEntityTargetTest<T> where T : IIncrementalGenerator, new()
{
    protected readonly GeneratorTestDriver<T> TestDriver = new(new T(), Console.WriteLine);

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