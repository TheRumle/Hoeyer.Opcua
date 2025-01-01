using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.IncrementalProvider;

internal record struct UnloadedIncrementalValuesProvider<T>(IncrementalValuesProvider<T> source)
{
    public IncrementalValuesProvider<T> Source { get; private set; } = source;

    public UnloadedIncrementalValuesProvider<T> Where(Func<T, bool> predicate) =>
        new(source.Where(predicate));
    
    public UnloadedIncrementalValuesProvider<TOut> Select<TOut>(Func<T, CancellationToken, TOut> selector) where TOut : BaseTypeDeclarationSyntax => new(source.Select(selector));

    public UnloadedIncrementalValuesProvider<TOut> SelectMany<TOut>(Func<T, CancellationToken, IEnumerable<TOut>> selector) where TOut : BaseTypeDeclarationSyntax => new(source.SelectMany(selector));
}