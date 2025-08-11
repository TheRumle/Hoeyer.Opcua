using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[InheritsTests]
public abstract class GeneratorWithAgentTargetTest<T> where T : IIncrementalGenerator, new()
{
    protected readonly GeneratorTestDriver<T> TestDriver = new(new T(), Console.WriteLine);

    [Test]
    [ValidAgentSourceCodeGenerator]
    [DisplayName("Can generate valid syntax tree for '$agentSourceCode'")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateValidSyntaxTrees(AgentSourceCode agentSourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(agentSourceCode.SourceCodeString);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");
    }

    [Test]
    [ValidAgentSourceCodeGenerator]
    [DisplayName("Generates valid syntax tree for $sourceCode")]
    public async Task WhenGivenValidSourceCode_ProducesValidSyntaxTree(AgentSourceCode sourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(sourceCode.SourceCodeString);
        var syntaxTree = CSharpSyntaxTree.ParseText(generationResult.SourceCode);
        var diagnostics = syntaxTree.GetDiagnostics().ToList();
        await Assert.That(diagnostics).IsEmpty()
            .Because(
                $" the generated syntax trees should not have compilation errors: {string.Join('\n', diagnostics)}");
    }

    [Test]
    [ValidAgentSourceCodeGenerator]
    [DisplayName("Will not produce any diagnostic for valid class fixtures")]
    public async Task Generator_ShouldNeverProduceDiagnostics(AgentSourceCode agentSourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(agentSourceCode.SourceCodeString);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}