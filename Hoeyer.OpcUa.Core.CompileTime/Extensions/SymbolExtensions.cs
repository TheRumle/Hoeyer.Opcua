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

    public static bool IsNumericType(this ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Byte => true,
            SpecialType.System_SByte => true,
            SpecialType.System_Int16 => true,
            SpecialType.System_UInt16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_UInt32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_UInt64 => true,
            SpecialType.System_Single => true,
            SpecialType.System_Double => true,
            SpecialType.System_Decimal => true,
            _ => false
        };
    }
}