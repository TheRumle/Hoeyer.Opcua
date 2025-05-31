using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.EndToEndTest;

internal static sealed class SelectedNodeIds
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