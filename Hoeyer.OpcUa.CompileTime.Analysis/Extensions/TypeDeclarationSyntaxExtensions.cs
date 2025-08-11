using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public static bool IsAnnotatedAsOpcUaEntity(this ISymbol? symbol)
    {
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }


    public static bool IsAnnotatedAsOpcUaEntityBehaviour(this TypeDeclarationSyntax typeSyntax,
        SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityBehaviourAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static AttributeData? GetOpcUaEntityBehaviourAttribute(this TypeDeclarationSyntax typeSyntax,
        SemanticModel semanticModel) =>
        semanticModel
            .GetDeclaredSymbol(typeSyntax)
            ?.GetAttributes()
            .FirstOrDefault(IsOpcEntityBehaviourAttributeSymbol);


    private static bool IsOpcEntityBehaviourAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityBehaviourAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    private static bool IsOpcEntityAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());
}