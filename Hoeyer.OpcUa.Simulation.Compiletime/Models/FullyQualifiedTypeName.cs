using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

public record FullyQualifiedTypeName
{
    public FullyQualifiedTypeName(string fullyQualifiedType)
    {
        WithoutGlobalPrefix = fullyQualifiedType;
        WithGlobalPrefix = $"global::{WithoutGlobalPrefix}";
    }


    public FullyQualifiedTypeName(INamedTypeSymbol symbol)
    {
        WithoutGlobalPrefix = symbol.ToDisplayString();
        WithGlobalPrefix = $"global::{WithoutGlobalPrefix}";
    }

    public string WithoutGlobalPrefix { get; }

    public string WithGlobalPrefix { get; }

    public bool Matches(string other) => WithGlobalPrefix.Equals(other) || WithoutGlobalPrefix.Equals(other);
}