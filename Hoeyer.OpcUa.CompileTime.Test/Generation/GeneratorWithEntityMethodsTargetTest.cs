using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

public abstract class GeneratorWithEntityMethodsTargetTest<T> where T : IIncrementalGenerator, new()
{
    protected readonly GeneratorTestDriver<T> TestDriver = new(new T(), Console.WriteLine);

    [Test]
    [EntityServiceInterfaceGenerator]
    [DisplayName("Can generate valid syntax tree for $serviceInterface")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateValidSyntaxTrees(
        ServiceInterfaceSourceCode serviceInterface)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(serviceInterface.AllSourceCode);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");
    }

    [Test]
    [EntityServiceInterfaceGenerator]
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
    [EntityServiceInterfaceGenerator]
    [DisplayName("Will not produce any diagnostic for valid fixtures")]
    public async Task Generator_ShouldNeverProduceDiagnostics(ServiceInterfaceSourceCode entitySourceCode)
    {
        GeneratorResult generationResult = TestDriver.RunGeneratorOn(entitySourceCode.SourceCodeString);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}