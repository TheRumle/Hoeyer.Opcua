using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class TypeSymbolExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this ISymbol symbol) =>
        symbol.GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

    extension(ITypeSymbol? symbol)
    {
        public AttributeData? GetOpcMethodAttribute() =>
            symbol?
                .GetAttributes()
                .FirstOrDefault(IsOpcMethodAttributeSymbol);

        public AttributeData? GetOpcArgsAttribute() =>
            symbol?
                .GetAttributes()
                .FirstOrDefault(IsOpcMethodArgsAttributeSymbol);
    }

    public static bool IsOpcEntityAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static bool IsOpcMethodAttributeSymbol(this AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityBehaviourAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static bool IsOpcMethodArgsAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute.OpcMethodArgumentsAttribute.WithGlobalPrefix.Equals(
            x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(DisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}