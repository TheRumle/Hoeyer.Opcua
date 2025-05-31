using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.CodeLoading;

public sealed class CompilationFactory(string compilationName, Action<string>? log = null)
{
    [Pure]
    public CSharpCompilation CreateCompilation(params IEnumerable<SyntaxTree> trees)
    {
        IEnumerable<SyntaxTree> syntaxTrees = trees as SyntaxTree[] ?? trees.ToArray();
        var referencedAssemblies = AssemblyLoader.CoreMetadataReferences;
        var compilation = CSharpCompilation.Create(compilationName,
            syntaxTrees,
            referencedAssemblies,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


        return compilation;
    }

    [Pure]
    public CSharpCompilation CreateCompilation(string sourceCode, SyntaxTree additional)
    {
        var referencedAssemblies = AssemblyLoader.CoreMetadataReferences;
        var compilation = CSharpCompilation.Create(compilationName,
            [CSharpSyntaxTree.ParseText(sourceCode), additional], referencedAssemblies,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation;
    }
}