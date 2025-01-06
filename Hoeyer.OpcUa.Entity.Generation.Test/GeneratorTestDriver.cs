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
            references: [ MetadataReference.CreateFromFile(typeof(object).Assembly.Location), .._necessaryReferences],
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

    private static PortableExecutableReference[] GetMetadataReferences()
    {
        HashSet<Assembly> initials = new()
        {
            typeof(T).Assembly,                        // The assembly containing the type T
            typeof(object).Assembly,                  // Core assembly (mscorlib or System.Private.CoreLib)
            typeof(OpcUaEntityAttribute).Assembly,    // Custom attribute assembly
            typeof(Attribute).Assembly                // System.Runtime (for Attribute type)
        };
        
        var references = initials
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Union(initials
                .SelectMany(a => a.GetReferencedAssemblies())
                .Select(assemblyName => MetadataReference.CreateFromFile(Assembly.Load(assemblyName).Location)))
            .ToList();

        // Ensure System.Runtime is added explicitly
        var systemRuntimeLocation = Assembly.Load("System.Runtime").Location;
        references.Add(MetadataReference.CreateFromFile(systemRuntimeLocation));

        return references.ToArray();
    }
}