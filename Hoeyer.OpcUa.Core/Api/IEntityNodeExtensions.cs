namespace Hoeyer.OpcUa.Core.Api;

public static class IEntityNodeExtensions
{
    public static EntityNodeStructure ToStructureOnly(this IEntityNode node) =>
        new(node);
}