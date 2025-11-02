using System.Linq;
using Hoeyer.OpcUa.Client.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Extensions;

public static class AttributeExtensions
{
    public static INamedTypeSymbol? GetEntityFromGenericArgument(this AttributeSyntax attribute, SemanticModel model)
    {
        SymbolInfo symbolInfo = model.GetSymbolInfo(attribute);
        if (symbolInfo.Symbol is not IMethodSymbol attributeConstructor) return null;

        INamedTypeSymbol? attributeType = attributeConstructor.ContainingType;


        if (attributeType is not INamedTypeSymbol { IsGenericType: true, Arity: 1 } namedType) return null;

        if (namedType.OriginalDefinition.ToDisplayString() == WellKnown.FullyQualifiedAttribute
                .GenericEntityBehaviourAttribute.WithoutGlobalPrefix ||
            namedType.OriginalDefinition.ToDisplayString() == WellKnown.FullyQualifiedAttribute
                .GenericEntityBehaviourAttribute.WithGlobalPrefix)
        {
            return namedType
                .TypeArguments
                .Where(type => type.IsAnnotatedAsOpcUaEntity())
                .OfType<INamedTypeSymbol>()
                .First();
        }

        return null;
    }

    public static AttributeData? GetOpcMethodAttribute(this ITypeSymbol? symbol) =>
        symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcMethodAttributeSymbol);

    public static bool IsOpcMethodAttributeSymbol(this AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityBehaviourAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());


    public static INamedTypeSymbol? GetEntityFromGenericAttributeArgument(this INamedTypeSymbol symbol)
    {
        return symbol
            .GetOpcMethodAttribute()?
            .AttributeClass?
            .TypeArguments
            .Where(type => type.IsAnnotatedAsOpcUaEntity())
            .OfType<INamedTypeSymbol>()
            .FirstOrDefault();
    }
}