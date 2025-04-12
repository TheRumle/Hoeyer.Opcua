using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

public record struct GeneratedClass<T>(
    CompilationUnitSyntax? CompilationUnit,
    TypeDeclarationSyntax? TypeDeclaration,
    T Origin) where T : SyntaxNode
{
    public bool IsSuccess => CompilationUnit is null || TypeDeclaration is not null;
    public bool IsFailure => !IsSuccess;

    public Location OriginLocation => Origin.GetLocation();

    public void AddToContext(SourceProductionContext context)
    {
        context.AddSource(TypeDeclaration!.Identifier.Text.TrimEnd() + ".g.cs",
            CompilationUnit!.NormalizeWhitespace().ToString());
    }
}