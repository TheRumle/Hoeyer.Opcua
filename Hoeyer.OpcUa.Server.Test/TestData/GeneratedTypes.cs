using Hoeyer.OpcUa.Core.Entity;

namespace Hoeyer.OpcUa.Server.Test.TestData;

public static class GeneratedTypes
{
    public static IReadOnlySet<IEntityNodeCreator> EntityNodeCreators { get; } = typeof(GeneratedTypes).Assembly
        .GetTypes()
        .Where(type => typeof(IEntityNodeCreator).IsAssignableFrom(type) &&
                       type is { IsInterface: false, IsAbstract: false })
        .Where(e => e.GetConstructor(Type.EmptyTypes) != null)
        .Select(Activator.CreateInstance)
        .Cast<IEntityNodeCreator>()
        .ToHashSet();
}