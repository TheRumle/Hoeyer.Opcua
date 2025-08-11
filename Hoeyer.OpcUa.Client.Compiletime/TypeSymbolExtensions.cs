using System.Linq;
using Hoeyer.OpcUa.Client.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Client.SourceGeneration;

public static class TypeSymbolExtensions
{
    public static bool IsAnnotatedAsOpcUaAgent(this ISymbol? symbol)
    {
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcAgentAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static bool IsOpcAgentAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .AgentAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(DisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}