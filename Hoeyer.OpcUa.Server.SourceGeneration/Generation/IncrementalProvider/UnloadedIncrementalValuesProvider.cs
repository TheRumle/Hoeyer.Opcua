using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;

internal record struct UnloadedIncrementalValuesProvider<T>(IncrementalValuesProvider<T> source)
{
    public IncrementalValuesProvider<T> Source { get; private set; } = source;

    public UnloadedIncrementalValuesProvider<T> Where(Func<T, bool> predicate)
    {
        return new UnloadedIncrementalValuesProvider<T>(source.Where(predicate));
    }

    public UnloadedIncrementalValuesProvider<TOut> Select<TOut>(Func<T, CancellationToken, TOut> selector)
    {
        return new UnloadedIncrementalValuesProvider<TOut>(source.Select(selector));
    }

    public UnloadedIncrementalValuesProvider<TOut> SelectMany<TOut>(
        Func<T, CancellationToken, IEnumerable<TOut>> selector)
    {
        return new UnloadedIncrementalValuesProvider<TOut>(source.SelectMany(selector));
    }

    public IncrementalValueProvider<ImmutableArray<T>> Collect()
    {
        return source.Collect();
    }
}