using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Core.Extensions.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

internal sealed record OpcMethodTypeInfo : IOpcTypeInfo
{
    public OpcMethodTypeInfo(BaseObjectState parent, string methodName, Type? returnType,
        IEnumerable<ParameterInfo> arguments)
        : this(parent, methodName, returnType, arguments.Select(e => (Type: e.ParameterType, name: e.Name)))
    {
    }

    public OpcMethodTypeInfo(BaseObjectState parent, string methodName, Type? returnType,
        IEnumerable<(Type type, string name)> arguments)
    {
        var opcMethodName = methodName;
        var method = new MethodState(parent)
        {
            NodeId = new NodeId(opcMethodName, parent.NodeId.NamespaceIndex),
            SymbolicName = opcMethodName,
            BrowseName = opcMethodName,
            DisplayName = opcMethodName,
            AccessRestrictions = AccessRestrictionType.None,
            Executable = true,
            UserExecutable = true,
        };

        CreateInputArguments(methodName, arguments, parent, method);
        if (returnType is not null)
        {
            CreateReturnValueNode(methodName, returnType, parent, method);
        }

        Method = method;
    }

    public MethodState Method { get; set; }

    public BaseInstanceState InstanceState => Method;

    private static void CreateInputArguments(string methodName, IEnumerable<(Type type, string name)> arguments,
        BaseObjectState parent, MethodState method)
    {
        var inputArguments = arguments.Select(e =>
        {
            var (id, rank) = e.type.GetOpcTypeInfo();
            return new Argument
            {
                Name = e.name,
                DataType = id,
                ValueRank = rank
            };
        }).ToArray();

        var inputArgument = new PropertyState<Argument[]>(method)
        {
            NodeId = new NodeId(methodName + "InputArgs", parent.NodeId.NamespaceIndex),
            BrowseName = BrowseNames.InputArguments,
            DisplayName = BrowseNames.InputArguments,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            DataType = DataTypeIds.Argument,
            ValueRank = ValueRanks.OneDimension,
            Value = inputArguments
        };
        method.InputArguments = inputArgument;
        method.AddReference(ReferenceTypeIds.HasProperty, false, inputArgument.NodeId);
    }

    private void CreateReturnValueNode(string methodName, Type returnType, BaseObjectState parent, MethodState method)
    {
        var (typeId, valueRank) = returnType.GetOpcTypeInfo();

        Argument[] value =
        [
            new()
            {
                Name = "Result",
                DataType = typeId,
                ValueRank = valueRank
            }
        ];

        var outputArgument = new PropertyState<Argument[]>(method)
        {
            NodeId = new NodeId(methodName + "Result", parent.NodeId.NamespaceIndex),
            BrowseName = BrowseNames.OutputArguments,
            DisplayName = BrowseNames.OutputArguments,
            TypeDefinitionId = VariableTypeIds.PropertyType,
            DataType = DataTypeIds.Argument,
            ValueRank = ValueRanks.Scalar,
            Value = value
        };
        method.OutputArguments = outputArgument;
        method.AddReference(ReferenceTypeIds.HasProperty, false, outputArgument.NodeId);
    }
}