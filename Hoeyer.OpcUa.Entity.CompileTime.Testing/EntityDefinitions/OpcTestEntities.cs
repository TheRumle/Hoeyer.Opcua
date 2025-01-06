﻿using System.Collections.Immutable;
using Hoeyer.Common.Extensions.Reflection;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class OpcTestEntities 
{
    public static readonly string CurrentNamespace = typeof(OpcTestEntities).Namespace!;
    public static readonly string CurrentPath = CurrentNamespace.Replace('.', Path.DirectorySeparatorChar);
    
    public static readonly ImmutableHashSet<Type> TestEntities = typeof(OpcTestEntities).Assembly
        .GetTypes()
        .Where(type => type.FullName != default && type.FullName.Contains(CurrentNamespace))
        .Where(type => type is { IsAbstract: false, IsInterface: false })
        .Where(type => type.IsAnnotatedWith<OpcUaEntityAttribute>())
        .ToImmutableHashSet();

    public static ImmutableHashSet<Type> PositiveEntities => TestEntities
        .Where(type => type.FullName!.Contains($"{nameof(EntityDefinitions)}.Correct"))
        .ToImmutableHashSet();
    
    public static ImmutableHashSet<Type> NegativeEntities => TestEntities
        .Where(type => type.FullName!.Contains($"{nameof(EntityDefinitions)}.{nameof(Incorrect)}"))
        .ToImmutableHashSet();
}