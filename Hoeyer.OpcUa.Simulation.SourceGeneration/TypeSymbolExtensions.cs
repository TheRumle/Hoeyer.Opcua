using System.Linq;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class TypeSymbolExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this ISymbol? symbol)
    {
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcEntityAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static bool IsOpcEntityAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .EntityAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(DisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}