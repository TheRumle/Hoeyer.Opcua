using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.EntityDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

public abstract class GeneratorWithEntityMethodsTargetTest(IIncrementalGenerator generator)
{
    private readonly GeneratorTestDriver _testDriver = new(generator, Console.WriteLine);

    [Test]
    [EntityServiceInterfaceGenerator]
    [DisplayName("Can generate valid syntax tree for $serviceInterface")]
    public async Task WhenGiven_CorrectSourceCodeInfo_ShouldGenerateValidSyntaxTrees(
        ServiceInterfaceSourceCode serviceInterface)
    {
        var generationResult = _testDriver.RunGeneratorOn(serviceInterface.AllSourceCode);
        await Assert.That(generationResult.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");
    }

    [Test]
    [EntityServiceInterfaceGenerator]
    [DisplayName("Generates valid syntax tree for $serviceInterface")]
    public async Task WhenGivenValidSourceCode_ProducesValidSyntaxTree(ServiceInterfaceSourceCode serviceInterface)
    {
        var generationResult = _testDriver.RunGeneratorOn(serviceInterface.AllSourceCode);
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
        var generationResult = _testDriver.RunGeneratorOn(entitySourceCode.SourceCodeString);
        await Assert.That(generationResult.Errors).IsEmpty().Because(
            "The generator should not be responsible for analyzing source code, only production of generated code.");
    }
}