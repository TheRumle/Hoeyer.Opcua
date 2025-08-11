using System.Linq;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class TypeSymbolExtensions
{
    public static bool IsAnnotatedAsOpcUaAgent(this ISymbol symbol) =>
        symbol.GetAttributes()
            .Any(IsOpcAgentAttributeSymbol);

    public static bool IsAnnotatedAsOpcMethodArgs(this ITypeSymbol symbol) => GetOpcArgsAttribute(symbol) != null;

    public static AttributeData? GetOpcAgentAttribute(this ITypeSymbol? symbol) =>
        symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcAgentAttributeSymbol);


    public static AttributeData? GetOpcArgsAttribute(this ITypeSymbol? symbol) =>
        symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcMethodArgsAttributeSymbol);

    public static bool IsOpcAgentAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .AgentAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static bool IsOpcMethodArgsAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute.OpcMethodArgumentsAttribute.WithGlobalPrefix.Equals(
            x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(DisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}