using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public static class ExtensionMethods
{
    public static ExtensionObject ToExtensionObject(this IEntityNode node)
    {
        var structure = new Dictionary<string, object>();
        foreach (var property in node.PropertyStates)
        {
            structure[property.BrowseName.Name] = property.Value;
        }

        var encodedStructure = new ExtensionObject
        {
            Body = new Variant(structure),
            TypeId = ExpandedNodeId.Null
        };
        return encodedStructure;
    }
    
}