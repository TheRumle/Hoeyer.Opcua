namespace Hoeyer.Common.Architecture;

public sealed record AssemblyMarker(Type Marker)
{
    public Type Marker { get; } = Marker;

    public Type[] TypesInAssembly { get; } = Marker.Assembly.GetExportedTypes();
}