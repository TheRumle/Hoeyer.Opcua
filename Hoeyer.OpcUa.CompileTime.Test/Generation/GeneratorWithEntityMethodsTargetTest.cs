using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

public abstract class GeneratorWithAgentMethodsTargetTest<T> where T : IIncrementalGenerator, new()
{
    protected readonly GeneratorTestDriver<T> TestDriver = new(new T(), Console.WriteLine);

    [Test]
    [AgentServiceInterfaceGenerator]
    [DisplayName("Can generate valid syntax tree for $serviceInterface")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateValidSyntaxTrees(
        ServiceInterfaceSourceCode serviceInterface)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(serviceInterface.AllSourceCode);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");
    }

    [Test]
    [AgentServiceInterfaceGenerator]
    [DisplayName("Generates valid syntax tree for $serviceInterface")]
    public async Task WhenGivenValidSourceCode_ProducesValidSyntaxTree(ServiceInterfaceSourceCode serviceInterface)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(serviceInterface.AllSourceCode);
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(generationResult.SourceCode);
        List<Diagnostic> diagnostics = syntaxTree.GetDiagnostics().ToList();
        await Assert.That(diagnostics).IsEmpty()
            .Because(
                $" the generated syntax trees should not have compilation errors: {string.Join('\n', diagnostics)}");
    }


    [Test]
    [AgentServiceInterfaceGenerator]
    [DisplayName("Will not produce any diagnostic for valid fixtures")]
    public async Task Generator_ShouldNeverProduceDiagnostics(ServiceInterfaceSourceCode agentSourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(agentSourceCode.SourceCodeString);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}