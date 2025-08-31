using Hoeyer.OpcUa.Core.CompileTime.CodeDomain;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Core.CompileTime.Extensions;

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