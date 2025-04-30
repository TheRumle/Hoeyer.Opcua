using System.Diagnostics.CodeAnalysis;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Api;

[SuppressMessage("Maintainability", "S3265", Justification = "The Opc.Ua library is not designed using Flags - unfortunately")]
public static class NodeClassFilters
{
    public const NodeClass Any = NodeClass.Object
                           | NodeClass.ObjectType
                           | NodeClass.ReferenceType
                           | NodeClass.VariableType
                           | NodeClass.Variable
                           | NodeClass.DataType
                           | NodeClass.ReferenceType
                           | NodeClass.Method
                           | NodeClass.Unspecified;
    
    public const NodeClass EntityData = NodeClass.Method | NodeClass.Variable | NodeClass.Object;
    public const NodeClass Entity = NodeClass.ReferenceType  | NodeClass.Object | NodeClass.ObjectType;

}