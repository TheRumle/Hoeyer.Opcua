using Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class SymbolExtensions
{

    
    public static string GloballyQualifiedNonGeneric(this ISymbol typeSymbol) =>
        typeSymbol.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix);

    public static string ToFullyQualifiedTypeName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
}