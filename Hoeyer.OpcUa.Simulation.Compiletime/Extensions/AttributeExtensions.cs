using System.Linq;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Extensions;

public static class AttributeExtensions
{
    public static INamedTypeSymbol? GetAgentFromGenericArgument(this AttributeSyntax attribute, SemanticModel model)
    {
        SymbolInfo symbolInfo = model.GetSymbolInfo(attribute);
        if (symbolInfo.Symbol is not IMethodSymbol attributeConstructor) return null;

        INamedTypeSymbol? attributeType = attributeConstructor.ContainingType;


        if (attributeType is not INamedTypeSymbol { IsGenericType: true, Arity: >= 1 } namedType) return null;

        if (namedType.OriginalDefinition.ToDisplayString() == WellKnown.FullyQualifiedAttribute
                .GenericAgentBehaviourAttribute.WithoutGlobalPrefix ||
            namedType.OriginalDefinition.ToDisplayString() == WellKnown.FullyQualifiedAttribute
                .GenericAgentBehaviourAttribute.WithGlobalPrefix)
        {
            return namedType
                .TypeArguments
                .Where(type => type.IsAnnotatedAsOpcUaAgent())
                .OfType<INamedTypeSymbol>()
                .First();
        }

        return null;
    }
}