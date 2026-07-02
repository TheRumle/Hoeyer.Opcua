using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

namespace Hoeyer.OpcUa.Core.Test.Application;

public record struct GenericImplementation<TService>(
    TService Service,
    IBrowseNameCollection BrowseNameCollection,
    IBehaviourTypeModel Behaviour)
{
    public override string ToString() => $"{typeof(TService).Name} ({BrowseNameCollection.EntityName})";
}