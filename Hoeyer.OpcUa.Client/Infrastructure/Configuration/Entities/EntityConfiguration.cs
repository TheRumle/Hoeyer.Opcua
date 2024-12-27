using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Builder;
using Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities.Property;

namespace Hoeyer.OpcUa.Client.Infrastructure.Configuration.Entities;

[SuppressMessage("Maintainability", "S2326:Unused parameters should be removed", Justification = "The generic value is used for service registrering, and the type specifies the entity connected to this configuration", Scope = "member")]
public sealed record EntityConfiguration<T>(RootIdentity RootIdentity)
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