using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;

public sealed record TypeContext(SemanticModel SemanticModel, TypeDeclarationSyntax Node)
{
    public SemanticModel SemanticModel { get; } = SemanticModel;
    public TypeDeclarationSyntax Node { get; } = Node;
}