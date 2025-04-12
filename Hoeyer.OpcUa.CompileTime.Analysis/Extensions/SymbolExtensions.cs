using Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class SymbolExtensions
{
    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol)
    {
        return typeSymbol.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);
    }

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}