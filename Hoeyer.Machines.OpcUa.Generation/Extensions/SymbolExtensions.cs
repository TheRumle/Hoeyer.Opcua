using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.Extensions;

internal static class SymbolExtensions
{
    public static bool IsContainedInNamespace(this INamedTypeSymbol symbol, string @namespace)
    {
        return symbol.ContainingNamespace.ToDisplayString().StartsWith(@namespace);
    }
}