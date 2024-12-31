using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.Configuration;

public readonly struct TypeContext(SemanticModel semanticModel, BaseTypeDeclarationSyntax node)
{
    public SemanticModel SemanticModel { get; } = semanticModel;
    public BaseTypeDeclarationSyntax Node { get; } = node;
    public INamespaceSymbol GetNamespace => SemanticModel.GetDeclaredSymbol(Node)!.ContainingNamespace;
}