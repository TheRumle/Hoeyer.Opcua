using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Constants;

public static class Locations
{
    public static readonly NamespaceDeclarationSyntax GeneratedPlacement = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Hoeyer.OpcUa.Server.Generated"));

    public static readonly UsingDirectiveSyntax ObservabilityNamespace = SyntaxFactory.UsingDirective(
        SyntaxFactory.ParseName("Hoeyer.OpcUa.Core.Observation")
    );

}