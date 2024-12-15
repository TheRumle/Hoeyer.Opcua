using System;
using System.Collections.Generic;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration.Builder;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;

public record EntityConfiguration<TEntity>(RootIdentity RootIdentity)
{
    private readonly List<PropertyConfiguration> _propertyConfigurations = [];
    public IReadOnlyCollection<PropertyConfiguration> PropertyConfigurations => _propertyConfigurations;
    public RootIdentity RootIdentity { get; } = RootIdentity;
    public Type InstanceFactoryType { get; internal set; } = null!;

    public PropertyConfiguration AddPropertyConfiguration(PropertyInfo propertyInfo, NodeIdConfiguration node)
    {
        var a = new PropertyConfiguration(propertyInfo, node, RootIdentity);
        _propertyConfigurations.Add(a);
        return a;
    }
}