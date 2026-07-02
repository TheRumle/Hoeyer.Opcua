namespace Hoeyer.OpcUa.Core.Abstractions;

public static class IEntityNodeExtensions
{
    public static EntityNodeStructure ToStructureOnly(this IEntityNode node) =>
        new(node);
}