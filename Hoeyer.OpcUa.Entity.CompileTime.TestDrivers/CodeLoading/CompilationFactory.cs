using System.Diagnostics.Contracts;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;

public sealed class CompilationFactory(string compilationName, Action<string>? log = null)
{
    private readonly Action<string> Log = log == null ? _ => { } : log;

    [Pure]
    public CSharpCompilation CreateCompilation(EntitySourceCode entitySourceCode)
    {
        var sourceCode = entitySourceCode.SourceCodeString;
        Log(sourceCode);

        var referencedAssemblies = AssemblyLoader.GetMetaReferencesContainedIn(entitySourceCode.Type)
            .Union(AssemblyLoader.CoreMetadataReferences);


        var compilation = CSharpCompilation.Create(compilationName,
            [CSharpSyntaxTree.ParseText(sourceCode)],
            referencedAssemblies,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        if (compilation.GetDiagnostics().Any())
            Log("The original compilation of the source code had errors! \n" +
                string.Join("\n", compilation.GetDiagnostics()));

        return compilation;
    }
}