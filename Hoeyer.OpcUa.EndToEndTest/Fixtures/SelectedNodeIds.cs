using Hoeyer.Common.Extensions.Collection;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest.Fixtures;

internal static class SelectedNodeIds
{
    public static IEnumerable<Func<NodeId>> PresentObjects()
    {
        IEnumerable<NodeId> ids =
        [
            ObjectIds.Dictionaries, ObjectIds.Aliases, ObjectIds.Locations, ObjectIds.Quantities, ObjectIds.Resources
        ];
        return ids.SelectFunc();
    }
}