namespace Hoeyer.Common.Architecture;

public sealed class ServiceAssemblyMarkerCollection(AssemblyMarker[] assemblyMarkers) : List<AssemblyMarker>(assemblyMarkers)
{
    public ServiceAssemblyMarkerCollection(AssemblyMarker m) : this([m]){}
}
