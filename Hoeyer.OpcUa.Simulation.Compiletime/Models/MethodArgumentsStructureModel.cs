using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

public record struct MethodArgumentsStructureModel
{
    public readonly INamedTypeSymbol Agent;
    public readonly INamedTypeSymbol InterfaceSymbol;
    public readonly IReadOnlyCollection<IMethodSymbol> Methods;

    public MethodArgumentsStructureModel(
        INamedTypeSymbol agent,
        INamedTypeSymbol interfaceSymbol,
        IEnumerable<IMethodSymbol> methods)
    {
        Agent = agent;
        InterfaceSymbol = interfaceSymbol;
        Methods = methods.ToList();
    }
}