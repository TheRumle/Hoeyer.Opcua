using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.CodeLoading;

public sealed class CompilationFactory(string compilationName, Action<string>? log = null)
{
    private readonly Action<string> _log = log == null ? _ => { } : log;

    [Pure]
    public CSharpCompilation CreateCompilation(EntitySourceCode entitySourceCode)
    {
        var sourceCode = entitySourceCode.SourceCodeString;
        _log(sourceCode);

        var referencedAssemblies = AssemblyLoader.CoreMetadataReferences;


        var compilation = CSharpCompilation.Create(compilationName,
            [CSharpSyntaxTree.ParseText(sourceCode)],
            referencedAssemblies,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        if (compilation.GetDiagnostics().Any())
        {
            _log("The original compilation of the source code had errors! \n" +
                 string.Join("\n", compilation.GetDiagnostics()));
        }

        return compilation;
    }
}