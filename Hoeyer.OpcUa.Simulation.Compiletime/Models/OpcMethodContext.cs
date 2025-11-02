using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

public record struct OpcMethodContext
{
    public readonly INamedTypeSymbol Entity;
    public readonly INamedTypeSymbol InterfaceSymbol;
    public readonly IReadOnlyCollection<IMethodSymbol> Methods;

    public OpcMethodContext(
        INamedTypeSymbol entity,
        INamedTypeSymbol interfaceSymbol,
        IEnumerable<IMethodSymbol> methods)
    {
        Entity = entity;
        InterfaceSymbol = interfaceSymbol;
        Methods = methods.ToList();
    }
}