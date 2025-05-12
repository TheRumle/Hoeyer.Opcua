using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }
    
    public static bool IsAnnotatedAsOpcUaEntityBehaviour(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityBehaviourAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static bool IsAnnotatedAsOpcUaEntity(this INamedTypeSymbol symbol)
    {
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    private static bool IsOpcEntityBehaviourAttributeSymbol(AttributeData x)
    {
        return WellKnown
            .FullyQualifiedAttribute
            .EntityBehaviourAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());
    }

    private static bool IsOpcEntityAttributeSymbol(AttributeData x)
    {
        return WellKnown.FullyQualifiedAttribute
            .EntityAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());
    }

    
    public static IEnumerable<PropertyDeclarationSyntax> GetOpcEntityPropertyDeclarations(
        this SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
        {
            return [];
        }

        return typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
    }
}