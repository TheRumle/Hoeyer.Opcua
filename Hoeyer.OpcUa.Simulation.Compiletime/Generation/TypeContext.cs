using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;

public sealed record TypeContext(SemanticModel SemanticModel, InterfaceDeclarationSyntax Node)
{
    public SemanticModel SemanticModel { get; } = SemanticModel;
    public InterfaceDeclarationSyntax Node { get; } = Node;
}