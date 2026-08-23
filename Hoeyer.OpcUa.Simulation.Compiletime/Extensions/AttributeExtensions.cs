using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Extensions;

public static class AttributeExtensions
{
    public static INamedTypeSymbol? GetEntityFromGenericArgument(this AttributeSyntax attribute, SemanticModel model)
    {
        var symbolInfo = model.GetSymbolInfo(attribute);
        if (symbolInfo.Symbol is not IMethodSymbol attributeConstructor)
        {
            return null;
        }

        var attributeType = attributeConstructor.ContainingType;


        if (attributeType is not INamedTypeSymbol { IsGenericType: true, Arity: >= 1 } namedType)
        {
            return null;
        }

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
}