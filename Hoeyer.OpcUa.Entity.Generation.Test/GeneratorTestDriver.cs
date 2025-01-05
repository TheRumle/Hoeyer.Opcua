using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentResults;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Entity.Generation.Test;

internal sealed class GeneratorTestDriver<T>(T generator) where T : IIncrementalGenerator
{
    
    [SuppressMessage("csharpsquid", "S3220", Justification = "Cannot match the suggested function which uses ISourceGenerator")]
    private CSharpGeneratorDriver Driver => CSharpGeneratorDriver.Create(generator);
    private readonly IEnumerable<PortableExecutableReference> _necessaryReferences = GetMetadataReferences();

    
    public Result<GenerationResult> RunGeneratorOnSourceCode(string sourceCode)
    {
        var compilation = CSharpCompilation.Create(nameof(GeneratorTestDriver<T>),
            syntaxTrees: [ CSharpSyntaxTree.ParseText(sourceCode)],
            references: _necessaryReferences,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        var originalDiagnostics = compilation.GetDiagnostics();
        if (originalDiagnostics.Any(e=>e.Severity==DiagnosticSeverity.Error)) 
            throw new ArgumentException($"Invalid source code: \n\"{sourceCode}\"\n The provided compilation cannot compile for the following reasons:" + string.Join(Environment.NewLine + "\t", originalDiagnostics));

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

    private static IEnumerable<PortableExecutableReference> GetMetadataReferences()
    {
        Queue<Assembly> queue = new();
        queue.Enqueue(typeof(T).Assembly);
        queue.Enqueue(typeof(OpcUaEntityAttribute).Assembly);
        
        HashSet<Assembly> visited = [];
        
        List<PortableExecutableReference> result = [];
        while (queue.TryDequeue(out var assembly))
        {
            if (!visited.Add(assembly)) continue;

            var newAssemblies = assembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .ToHashSet();

            foreach (var discovered in newAssemblies)
            {
                queue.Enqueue(discovered);
                result.Add(MetadataReference.CreateFromFile(discovered.Location));
            }
        }

        return result;
    }
}