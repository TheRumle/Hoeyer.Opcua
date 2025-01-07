using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

internal sealed class GeneratorTestDriver<T>(T generator, ITestOutputHelper? logger = null) where T : IIncrementalGenerator
{
    private readonly Action<string> Log = logger == null
        ? _ => { }
        : logger!.WriteLine;
    
    [SuppressMessage("csharpsquid", "S3220", Justification = "Cannot match the suggested function which uses ISourceGenerator")]
    private CSharpGeneratorDriver Driver => CSharpGeneratorDriver.Create(generator);

    
    public Result<GenerationResult> RunGeneratorOnSourceCode(SourceCodeInfo sourceCodeInfo)
    {
        var sourceCode = sourceCodeInfo.SourceCodeString;
        Log(sourceCode);
        
        var referencedAssemblies = AssemblyLoader.GetMetaReferencesContainedIn(sourceCodeInfo.Type)
            .Union(AssemblyLoader.BaseReferences);

        
        var compilation = CSharpCompilation.Create(nameof(GeneratorTestDriver<T>),
            syntaxTrees: [ CSharpSyntaxTree.ParseText(sourceCode)],
            references: referencedAssemblies,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        if (compilation.GetDiagnostics().Any()) 
            Log("The original compilation of the source code had errors! \n" + string.Join("\n", compilation.GetDiagnostics()));

        return RunCompilation(Driver, compilation);
    }
    

    private static GenerationResult RunCompilation(CSharpGeneratorDriver driver, Compilation compilation)
    {
        var compilationResult = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        return CreateResult(compilationResult, diagnostics, driver.GetTimingInfo());
    }

    private static GenerationResult CreateResult(
        GeneratorDriver compilationResult,
        ImmutableArray<Diagnostic> diagnostics,
        GeneratorDriverTimingInfo timingInfo)
    {
        return new GenerationResult (
            Diagnostics: diagnostics, 
            GeneratedTrees: compilationResult.GetRunResult().GeneratedTrees,
            TimingInformation: timingInfo.GeneratorTimes.First(e => e.Generator.GetGeneratorType() == typeof(T)).ElapsedTime);
    }
    
    
}