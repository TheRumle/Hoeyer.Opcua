using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Constants;

public static class Locations
{
    public static readonly NamespaceDeclarationSyntax GeneratedPlacement =
        SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Hoeyer.OpcUa.Server.Generated"));

    public static readonly ImmutableHashSet<UsingDirectiveSyntax> Utilities =
    [
        ..new List<string>
            {
                "Opc.Ua",
                "System",
                "System.Linq",
                "System.Collections",
                "Hoeyer.OpcUa.Core.Application.Translator",
                "System.Collections.Generic",
                "Hoeyer.OpcUa.Core",
                "Hoeyer.OpcUa.Core.Api",
            }
            .Select(e => SyntaxFactory.ParseName(e))
            .Select(SyntaxFactory.UsingDirective)
    ];
}