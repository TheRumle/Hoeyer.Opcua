using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;

public sealed class GeneratorTestDriver<T>(T generator, Action<string>? logger = null)
    where T : IIncrementalGenerator
{
    private readonly CompilationFactory _compilationFactory = new(nameof(GeneratorTestDriver<T>), logger);

    [SuppressMessage("csharpsquid", "S3220",
        Justification = "Cannot match the suggested function which uses ISourceGenerator")]
    private readonly CSharpGeneratorDriver _driver = CSharpGeneratorDriver.Create(generator);


    public GeneratorResult RunGeneratorOn(EntitySourceCode entitySourceCode)
    {
        var compilation = _compilationFactory.CreateCompilation(CSharpSyntaxTree.ParseText(entitySourceCode.SourceCodeString));
        var compilationResult = _driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        var result = CreateResult(compilationResult, diagnostics, _driver.GetTimingInfo());

        logger?.Invoke("Generated code: \n" + result.SourceCode);
        return result;
    }


    private static GeneratorResult CreateResult(GeneratorDriver compilationResult,
        ImmutableArray<Diagnostic> diagnostics,
        GeneratorDriverTimingInfo timingInfo)
    {
        return new GeneratorResult(
            diagnostics,
            compilationResult.GetRunResult().GeneratedTrees,
            timingInfo.GeneratorTimes.First(e => e.Generator.GetGeneratorType() == typeof(T))
                .ElapsedTime);
    }
}