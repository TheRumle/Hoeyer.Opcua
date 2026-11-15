using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation;

public record struct UnloadedIncrementalValuesProvider<T>(IncrementalValuesProvider<T> Source)
{
    public IncrementalValuesProvider<T> Source { get; private set; } = Source;


    public UnloadedIncrementalValuesProvider<TOut> Select<TOut>(Func<T, CancellationToken, TOut> selector) =>
        new(Source.Select(selector));
    
    public IncrementalValueProvider<ImmutableArray<T>> Collect() => Source.Collect();
}