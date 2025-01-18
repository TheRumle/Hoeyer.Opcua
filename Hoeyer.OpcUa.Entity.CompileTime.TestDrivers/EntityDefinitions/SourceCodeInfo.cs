using FluentResults;
using FluentResults.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

[Serializable]
public record SourceCodeInfo([field: NonSerialized] Type Type, string SourceCodeString)
{
    public async Task<TypeDeclarationSyntax> ToDeclarationSyntax()
    {
        var res = await Result.Try(() => CSharpSyntaxTree.ParseText(SourceCodeString).GetRootAsync())
            .Map( root => root.DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault());
        
        if (res.IsFailed) throw new InvalidOperationException(string.Join(",\n",res.Errors));
        return res.Value!;
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return Type.Name;
    }
}