using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class SymbolExtensions
{
    public static string GetFullNamespace(this INamedTypeSymbol symbol)
    {
        INamespaceSymbol? namespaceSymbol = symbol.ContainingNamespace;
        if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace) return string.Empty;

        var parts = new Stack<string>();
        while (!namespaceSymbol.IsGlobalNamespace)
        {
            parts.Push(namespaceSymbol.Name);
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        return string.Join(".", parts);
    }
}