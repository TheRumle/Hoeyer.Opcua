using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.CompileTime.CodeDomain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.CompileTime.Extensions;

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

    public static bool IsAnnotatedAsOpcUaAlarm(this EnumDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcAlarmAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static IEnumerable<AttributeData> GetOpcUaAlarmAttributes(this PropertyDeclarationSyntax typeSyntax,
        SemanticModel semanticModel) =>
        semanticModel
            .GetDeclaredSymbol(typeSyntax)
            ?.GetAttributes()
            .Where(IsOpcAlarmAttributeSymbol)
        ?? [];

    public static bool IsOpcAlarmAttributeSymbol(this AttributeData attribute) =>
        IsTypeInheritingFrom(attribute.AttributeClass, WellKnown.FullyQualifiedAttribute.AlarmAttribute);

    public static bool IsLegalRangeAlarm(this AttributeData attribute) => IsTypeInheritingFrom(attribute.AttributeClass,
        WellKnown.FullyQualifiedAttribute.LegalRangeAlarmAttribute);

    public static bool IsTypeInheritingFrom(this INamedTypeSymbol? clazz, FullyQualifiedTypeName target)
    {
        while (clazz != null)
        {
            if (target.WithGlobalPrefix.Equals(clazz.GloballyQualifiedNonGeneric()))
            {
                return true;
            }

            clazz = clazz.BaseType;
        }

        return false;
    }
}