using System;
using Hoeyer.OpcUa.Client.SourceGeneration.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Constants;

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
            serviceTypeReference = SyntaxFactory.GenericName(type.Name).WithTypeArgumentList(
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.OmittedTypeArgument()))
            );
        }
        else
        {
            serviceTypeReference =
                SyntaxFactory.ParseName(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }


        FullyQualifiedTypeName serviceAttribute = WellKnown.FullyQualifiedAttribute.OpcUaAgentServiceAttribute;

        return
            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(serviceAttribute.WithGlobalPrefix))
                .WithArgumentList(SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList<AttributeArgumentSyntax>([
                        SyntaxFactory.AttributeArgument(
                            SyntaxFactory.TypeOfExpression(serviceTypeReference)
                        ),
                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                        SyntaxFactory.AttributeArgument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(
                                    "global::Microsoft.Extensions.DependencyInjection.ServiceLifetime"),
                                SyntaxFactory.IdentifierName(scopeText)))
                    ])));
    }
}