using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.Configuration;

public readonly struct TypeContext<T>(SemanticModel semanticModel, T node) 
where T : BaseTypeDeclarationSyntax
{
    public SemanticModel SemanticModel { get; } = semanticModel;
    public T Node { get; } = node;
    public INamespaceSymbol GetNamespace => SemanticModel.GetDeclaredSymbol(Node)!.ContainingNamespace;
}