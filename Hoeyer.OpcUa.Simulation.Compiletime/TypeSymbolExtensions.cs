using System.Linq;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class TypeSymbolExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this ISymbol symbol) =>
        symbol.GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

    public static bool IsAnnotatedAsOpcMethodArgs(this ITypeSymbol symbol) => GetOpcArgsAttribute(symbol) != null;

    public static AttributeData? GetOpcEntityAttribute(this ITypeSymbol? symbol) =>
        symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcEntityAttributeSymbol);


    public static AttributeData? GetOpcArgsAttribute(this ITypeSymbol? symbol) =>
        symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcMethodArgsAttributeSymbol);

    public static bool IsOpcEntityAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static bool IsOpcMethodArgsAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute.OpcMethodArgumentsAttribute.WithGlobalPrefix.Equals(
            x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(DisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}