using System.Collections.Generic;
using System.Reflection;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Property;

namespace Hoeyer.Machines.OpcUa.Configuration.Entity.Context;

public record OpcUaEntityConfiguration(RootIdentity RootIdentity)
{
    private readonly List<PropertyConfiguration> _propertyConfigurations = [];
    public IReadOnlyCollection<PropertyConfiguration> PropertyConfigurations => _propertyConfigurations;
    public RootIdentity RootIdentity { get; } = RootIdentity;
    public PropertyConfiguration AddPropertyConfiguration(PropertyInfo propertyInfo, NodeIdConfiguration node)
    {
        var a = new PropertyConfiguration(propertyInfo, node, RootIdentity);
        _propertyConfigurations.Add(a);
        return a;
    }
}