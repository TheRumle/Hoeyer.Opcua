using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Generation.IncrementalProvider;

internal record struct UnloadedIncrementalValuesProvider<T>(IncrementalValuesProvider<T> source)
{
    public IncrementalValuesProvider<T> Source { get; private set; } = source;

    public UnloadedIncrementalValuesProvider<T> Where(Func<T, bool> predicate) => new(source.Where(predicate));

    public UnloadedIncrementalValuesProvider<TOut> Select<TOut>(Func<T, CancellationToken, TOut> selector) =>
        new(source.Select(selector));

    public UnloadedIncrementalValuesProvider<TOut> SelectMany<TOut>(
        Func<T, CancellationToken, IEnumerable<TOut>> selector) =>
        new(source.SelectMany(selector));

    public IncrementalValueProvider<ImmutableArray<T>> Collect() => source.Collect();
}