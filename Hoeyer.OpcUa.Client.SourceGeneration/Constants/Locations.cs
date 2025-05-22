using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.Client.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

public static class OpcAttributeUsages
{
    public enum Scope
    {
        Singleton,
        Transient,
        Scoped
    }

    public static AttributeSyntax GetOpcUaServiceAttributeFor(INamedTypeSymbol type, Scope scope)
    {
        var scopeText = scope switch
        {
            Scope.Singleton => "Singleton",
            Scope.Transient => "Transient",
            Scope.Scoped => "Scoped",
            var _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };

        NameSyntax serviceTypeReference;
        if (type.Arity > 0)
        {
            serviceTypeReference = GenericName(type.Name).WithTypeArgumentList(
                TypeArgumentList(SingletonSeparatedList<TypeSyntax>(OmittedTypeArgument()))
            );
        }
        else
            serviceTypeReference = ParseName(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));


        FullyQualifiedTypeName serviceAttribute = WellKnown.FullyQualifiedAttribute.OpcUaEntityServiceAttribute;

        return
            Attribute(IdentifierName(serviceAttribute.WithGlobalPrefix))
                .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>([
                    AttributeArgument(
                        TypeOfExpression(serviceTypeReference)
                    ),
                    Token(SyntaxKind.CommaToken),
                    AttributeArgument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::Microsoft.Extensions.DependencyInjection.ServiceLifetime"),
                            IdentifierName(scopeText)))
                ])));
    }
}