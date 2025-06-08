using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Constants;

public static class Locations
{
    public static readonly NamespaceDeclarationSyntax GeneratedPlacement =
        NamespaceDeclaration(ParseName("Hoeyer.OpcUa.Core.Generated"));

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
                "Hoeyer.OpcUa.Core.Api"
            }
            .Select(e => ParseName(e))
            .Select(UsingDirective)
    ];
}