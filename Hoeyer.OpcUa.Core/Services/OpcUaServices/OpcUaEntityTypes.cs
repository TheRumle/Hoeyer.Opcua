using System;
using System.Collections.Frozen;
using System.Linq;
using Hoeyer.Common.Reflection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public static class OpcUaEntityTypes
{
    public static readonly FrozenSet<Type> Entities = (typeof(OpcUaEntityAttribute)
                                                           .GetTypesFromConsumingAssemblies()
                                                       ?? throw new InvalidOperationException(
                                                           "GetTypesFromConsumingAssemblies returned null"))
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToFrozenSet();
}