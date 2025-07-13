using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Simulation.Services;

/// <summary>
/// A container that holds dependency injection services for the simulation framework. Holds a scope for the services
/// </summary>
public sealed class SimulationServicesContainer(IServiceCollection collection) : IServiceProvider, IServiceCollection
{
    private bool _needNewScope = true;
    private IServiceScope Scope { get; set; } = collection.BuildServiceProvider().CreateScope();

    public IEnumerator<ServiceDescriptor> GetEnumerator() => collection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)collection).GetEnumerator();

    public void Add(ServiceDescriptor item)
    {
        collection.Add(item);
        _needNewScope = true;
    }

    public void Clear()
    {
        collection.Clear();
        _needNewScope = true;
    }

    public bool Contains(ServiceDescriptor item) => collection.Contains(item);

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(ServiceDescriptor item) => collection.Remove(item);
    public int Count => collection.Count;
    public bool IsReadOnly => collection.IsReadOnly;
    public int IndexOf(ServiceDescriptor item) => collection.IndexOf(item);

    public void Insert(int index, ServiceDescriptor item)
    {
        collection.Insert(index, item);
        _needNewScope = true;
    }

    public void RemoveAt(int index)
    {
        collection.RemoveAt(index);
        _needNewScope = true;
    }

    public ServiceDescriptor this[int index]
    {
        get => collection[index];
        set
        {
            collection[index] = value;
            _needNewScope = true;
        }
    }

    public object GetService(Type serviceType)
    {
        if (_needNewScope)
        {
            Scope = collection.BuildServiceProvider().CreateScope();
            _needNewScope = false;
        }

        return Scope.ServiceProvider.GetService(serviceType);
    }
}