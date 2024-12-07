namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal record LoadTypeRequest(string WantedResource, string ResourceLocation)
{
    public string WantedResource { get; } = WantedResource;
    public string ResourceLocation { get; } = ResourceLocation;
}