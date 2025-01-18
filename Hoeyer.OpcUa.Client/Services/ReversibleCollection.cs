using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hoeyer.OpcUa.Client.Services;

public sealed class ReversibleCollection(IServiceCollection collection) : IDisposable
{
    public List<Type> RegisteredTypes { get; } = new();

    /// <inheritdoc />
    public void Dispose()
    {
        Reverse();
    }

    public void AddSingleTon(Type t)
    {
        collection.AddSingleton(t);
        RegisteredTypes.Add(t);
    }

    public void AddTransient(Type service, Type impl)
    {
        collection.AddTransient(service, impl);
        RegisteredTypes.Add(service);
        RegisteredTypes.Add(impl);
    }

    public void Reverse()
    {
        foreach (var type in RegisteredTypes) collection.RemoveAll(type);
    }
}