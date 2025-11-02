using System;
using System.Collections.Generic;

namespace Hoeyer.Common.Architecture;

public sealed record AssemblyMarker(Type Marker)
{
    public Type Marker { get; } = Marker;

    public IEnumerable<Type> TypesInAssembly { get; } = Marker.Assembly.GetExportedTypes();
}