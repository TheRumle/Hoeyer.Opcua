using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

internal sealed record OpcMethodTypeInfo : IOpcTypeInfo
{
    public OpcMethodTypeInfo(BaseObjectState parent, string methodName, Type? returnType,
        IEnumerable<ParameterInfo> arguments)
        : this(parent, methodName, returnType, arguments.Select(e => (Type: e.ParameterType, name: e.Name)))
    {
        InstanceState = parent;
        throw new NotImplementedException("Not yet migrated to 2.0");
    }


    public BaseInstanceState InstanceState { get; private set; }
    public MethodState Method { get; set; }


    public OpcMethodTypeInfo(BaseObjectState parent, string methodName, Type? returnType,
        IEnumerable<(Type type, string name)> arguments)
    {
        var opcMethodName = methodName;
        var method = new MethodState(parent)
        {
            NodeId = new NodeId(opcMethodName, parent.NodeId.NamespaceIndex),
            SymbolicName = opcMethodName,
            BrowseName = new QualifiedName(opcMethodName),
            DisplayName = new LocalizedText(opcMethodName),
            AccessRestrictions = AccessRestrictionType.None,
            Executable = true,
            UserExecutable = true
        };
//
        //     CreateInputArguments(methodName, arguments, parent, method);
        //     if (returnType is not null)
        //     {
        //         CreateReturnValueNode(methodName, returnType, parent, method);
        //     }
//
        //     Method = method;
        // }
//
//
//
        // private static void CreateInputArguments(string methodName,
        //     IEnumerable<(Type type, string name)> arguments,
        //     BaseObjectState parent,
        //     MethodState method)
        // {
        //     var inputArguments = arguments.Select(arg =>
        //     {
        //         var data = OpcTypeInfoUtils.ConstructOpcTypeInfo(arg.type);
//
        //         return new Argument
        //         {
        //             Name = arg.name,
        //             DataType = data.TypeId,
        //             ValueRank = data.Rank
        //         };
        //     }).ToArray();
//
        //     var property = PropertyState<Argument[]>.With<MyArgumentBuilder>(
        //         method,
        //         inputArguments);
//
        //     property.NodeId = new NodeId(
        //         methodName + "InputArgs",
        //         parent.NodeId.NamespaceIndex);
//
        //     property.BrowseName = new QualifiedName(BrowseNames.InputArguments);
        //     property.DisplayName = new LocalizedText(BrowseNames.InputArguments);
        //     property.TypeDefinitionId = VariableTypeIds.PropertyType;
        //     property.DataType = DataTypeIds.Argument;
        //     property.ValueRank = ValueRanks.OneDimension;
        // }
//
        // private void CreateReturnValueNode(string methodName, Type returnType, BaseObjectState parent, MethodState method)
        // {
        //     var (_, valueRank, typeId) = OpcTypeInfoUtils.ConstructOpcTypeInfo(returnType);
//
        //     Argument[] value =
        //     [
        //         new()
        //         {
        //             Name = "Result",
        //             DataType = typeId,
        //             ValueRank = valueRank
        //         }
        //     ];
//
        //     var outputArgument = new PropertyState<Argument[]>(method)
        //     {
        //         NodeId = new NodeId(methodName + "Result", parent.NodeId.NamespaceIndex),
        //         BrowseName = new (BrowseNames.OutputArguments),
        //         DisplayName = new (BrowseNames.OutputArguments),
        //         TypeDefinitionId = VariableTypeIds.PropertyType,
        //         DataType = DataTypeIds.Argument,
        //         ValueRank = ValueRanks.Scalar,
        //         Value = value
        //     };
        //     method.OutputArguments = outputArgument;
        //     method.AddReference(ReferenceTypeIds.HasProperty, false, outputArgument.NodeId);
        // }
    }
}