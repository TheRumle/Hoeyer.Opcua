using System.Collections.Generic;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

internal interface IResourceLoader
{
    public IEnumerable<LoadableType> LoadResources();
}
