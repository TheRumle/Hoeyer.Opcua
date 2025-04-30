using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Constants;

public static class Locations
{
    public static readonly NamespaceDeclarationSyntax GeneratedPlacement =
        SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Hoeyer.OpcUa.Server.Generated"));

    public static readonly ImmutableArray<UsingDirectiveSyntax> Utilities =
    [
        ..new List<string>
            {
                "Opc.Ua",
                "System",
                "System.Linq",
                "System.Collections",
                "System.Collections.Generic",
                "Hoeyer.OpcUa.Core.Entity",
                "Hoeyer.OpcUa.Core",
                "Hoeyer.OpcUa.Core.Entity.Node",
                "Hoeyer.OpcUa.Core.Entity.State"
            }
            .Select(e => SyntaxFactory.ParseName(e))
            .Select(SyntaxFactory.UsingDirective)
    ];
}